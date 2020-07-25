using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FFT.Core.IO
{
    class FileExplorer
    {
        public static Configuration Config;
        public static Logger Log;

        static readonly TransferManager tm = new TransferManager();
        public static void HandlePacket(Client client, Packet packet)
        {
            try
            {
                if (Log == null || Log.isDisposed)
                {
                    Log = new Logger(client);
                }

                Debug($"Packet Received: {packet.PacketHeader}, Length {packet.Payload.Length}");

                switch (packet.PacketHeader)
                {
                    case PacketHeader.GetDrives:
                        GetDrivesPayload(client);
                        break;
                    case PacketHeader.GetDirectory:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        GetDirectory(client, packet.ToString()); 
                        break;
                    // THESE ARE INVERTED... 
                    // A Download from the UI is an upload here
                    // Upload from UI is a download here
                    case PacketHeader.DownloadFile:
                        tm.StartUpload(client, packet);
                        break;
                    case PacketHeader.UploadFile:
                        tm.StartDownload(client, packet);
                        break;
                    case PacketHeader.FileChunk:
                        tm.HandleFileChunk(client, packet);
                        break;
                    case PacketHeader.DirectoryDelete:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        DeleteDirectory(packet.ToString());
                        break;
                    case PacketHeader.DirectoryCreate:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        CreateDirectory(packet.ToString());
                        break;
                    case PacketHeader.DirectoryMove:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        MoveDirectory(packet);
                        break;
                    case PacketHeader.FileMove:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        MoveFile(packet);
                        break;
                    case PacketHeader.FileDelete:
                        IsProtected(packet.ToString()); // Permissions Check (Protected Directories)
                        DeleteFile(packet.ToString());
                        break;
                    case PacketHeader.CancelTransfer:
                        client.Send(Packet.Create(PacketHeader.CancelTransfer, tm.CancelTransfer(packet.ToString()).TransferId));
                        break;
                    case PacketHeader.GoodBye:
                        tm.CancelAllActiveTransfers();
                        Log?.Dispose();
                        break;
                    default:
                        break;
                }

                // Refresh ui after
                if (packet.PacketHeader >= PacketHeader.DirectoryDelete && packet.PacketHeader <= PacketHeader.FileBrowserException)
                {
                    Debug($"Packet Sent: {packet.PacketHeader}, Length {packet.Payload.Length}");
                    client.Send(packet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SendException(client, e);
            }
        }
        private static void MoveFile(Packet p)
        {
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string src = br.ReadString();
                    string dst = br.ReadString();
                    Log?.Info($"Moved File: {src} -> {dst}");
                    File.Move(src, dst);
                }
            }
        }

        private static void DeleteFile(string path)
        {
            Log?.Info($"Deleted File: {path}");
            File.Delete(path);
        }

        private static void MoveDirectory(Packet p)
        {
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string src = br.ReadString();
                    string dst = br.ReadString();
                    Log?.Info($"Moved Directory: {src} -> {dst}");
                    Directory.Move(src, dst);
                }
            }
        }

        private static void CreateDirectory(string path)
        {
            Log?.Info($"Created Directory: {path}");
            Directory.CreateDirectory(path);
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Log?.Info($"Deleted Directory: {path}");
                Directory.Delete(path, true);
            }
        }

        private static void SendException(Client c, Exception e)
        {
            Log?.Error(e.Message);
            c.Send(Packet.Create(PacketHeader.FileBrowserException, e.Message));
        }
          

        private static void GetDrivesPayload(Client client)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    int driveCount = DriveInfo.GetDrives().Where(d => d.IsReady).Count();
                    // Amount of drives
                    bw.Write(driveCount);

                    foreach(var di in DriveInfo.GetDrives().Where(d => d.IsReady))
                    {
                        try
                        {
                            bw.Write(di.Name);
                            bw.Write(di.VolumeLabel);
                            bw.Write(GetSize((double)di.TotalSize));
                            bw.Write(GetSize((double)di.TotalSize - (double)di.AvailableFreeSpace));
                            bw.Write(GetSize((double)di.TotalFreeSpace));
                            bw.Write(di.DriveFormat);
                            bw.Write(di.DriveType.ToString());
                            bw.Write((Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower().Contains(di.RootDirectory.Name.ToLower())));
                        } 
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                Log?.Info("Listing available drives...");
                client.Send(Packet.Create(PacketHeader.DrivesResponse, ms.ToArray()));
            }
        }

        private static void GetDirectory(Client client, string dir)
        {
            if (!Directory.Exists(dir))
            {
                // TODO: Check for directory shortcuts
                GetDrivesPayload(client);
            }
            else
            {
                Log?.Info($"Accessing Directory: {dir}");

                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        string root = dir.Remove(dir.LastIndexOf("\\"));
                        if (root.Length == 2)
                            root += "\\";

                        var directories = Directory.EnumerateDirectories(dir);
                        var files = Directory.EnumerateFiles(dir);

                        // This will serve as a place holder for the user to go "up" a directory
                        List<string[]> temp = new List<string[]>();
                        temp.Add(new string[]
                        {
                            "..",
                            root,
                            " ",
                            " ",
                            " "
                        });

                        // TODO: This can be refactored down to 2 loops instead of 4
                        // We can iterate the directory and write to the memory stream
                        // at the same time.
                        foreach (var d in directories)
                        {
                            try 
                            {
                                DirectoryInfo di = new DirectoryInfo(d);
                                var isProt = IsProtected(di.FullName, false) ? "*" : "";
                                temp.Add(new string[]
                                {
                                    di.Name+ isProt,
                                    di.FullName,
                                    di.GetFiles().Length + " FILES" ,
                                    di.CreationTime.ToString("dd/MM/yyyy HH:ss"),
                                    di.LastAccessTime.ToString("dd/MM/yyyy HH:ss")
                                });
                            }
                            catch (Exception e)
                            {
                                Debug(e.Message);
                                Console.WriteLine(e.Message);
                            }
                        }

                        bw.Write(temp.Count());
                        foreach(string[] t in temp)
                        {
                            for(int i = 0; i < t.Length; i++)
                            {
                                bw.Write(t[i]);
                            }
                        }


                        temp = new List<string[]>();
                        foreach(var f in files)
                        {
                            try
                            {

                                FileInfo fi = new FileInfo(f);
                                temp.Add(new string[]
                                {
                                    fi.Name,
                                    fi.FullName,
                                    GetSize((double)fi.Length),
                                    fi.CreationTime.ToString("dd/MM/yyyy HH:ss"),
                                    fi.LastAccessTime.ToString("dd/MM/yyyy HH:ss"),
                                    fi.Length.ToString()
                                });
                            }
                            catch (Exception e)
                            {
                                Debug(e.Message);
                                Console.WriteLine(e.Message);
                            }
                        }

                        bw.Write(temp.Count());
                        foreach (string[] t in temp)
                        {
                            for (int i = 0; i < t.Length; i++)
                            {
                                bw.Write(t[i]);
                            }
                        }
                    }

                    client.Send(Packet.Create(PacketHeader.DirectoryResponse, ms.ToArray()));
                }
            }
        }

        public static bool IsProtected(string dir, bool throwException = true)
        {
            var match = Config.ProtectedFolders.FirstOrDefault(d => dir.ToLower().StartsWith(d.ToLower()));

            if (match != null && throwException)
            {
                throw new Exception($"Access denied! '{dir}' is a protected directory.");
            }

            return match != null;
        }

        public static string GetSize(double size)
        {
            string rtn;
            rtn = string.Format("{0:0} bytes", size);
            if (size > 100)
            {
                size /= 1024; //kb
                rtn = string.Format("{0:0.00} KB", size);
            }
            if (size > 100)
            {
                size /= 1024; //mb
                rtn = string.Format("{0:0.00} MB", size);
            }
            if (size > 1000)
            {
                size /= 1024; // gb
                rtn = string.Format("{0:0.00} GB", size);
            }
            return rtn;
        }

        public static void Debug(string msg)
        {
            // Change this check to be in the logger class...
            if (Config.DebugMode)
            {
                Log?.Debug(msg);
            }
        }
    }
}
