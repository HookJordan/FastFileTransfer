using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.IO
{
    class FileExplorer
    {
        static readonly TransferManager tm = new TransferManager();
        
        public static void HandlePacket(Client client, Packet packet)
        {
            try
            {
                switch (packet.PacketHeader)
                {
                    case PacketHeader.GetDrives:
                        GetDrivesPayload(client);
                        break;
                    case PacketHeader.GetDirectory:
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
                        DeleteDirectory(packet.ToString());
                        break;
                    case PacketHeader.DirectoryCreate:
                        CreateDirectory(packet.ToString());
                        break;
                    case PacketHeader.DirectoryMove:
                        MoveDirectory(packet);
                        break;
                    case PacketHeader.FileMove:
                        MoveFile(packet);
                        break;
                    case PacketHeader.FileDelete:
                        DeleteFile(packet.ToString());
                        break;
                    case PacketHeader.CancelTransfer:
                        client.Send(Packet.Create(PacketHeader.CancelTransfer, tm.CancelTransfer(packet.ToString()).TransferId));
                        break;
                    default:
                        break;
                }

                // Refresh ui after
                if (packet.PacketHeader >= PacketHeader.DirectoryDelete && packet.PacketHeader <= PacketHeader.FileBrowserException)
                {
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

                    File.Move(src, dst);
                }
            }
        }

        private static void DeleteFile(string path)
        {
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

                    Directory.Move(src, dst);
                }
            }
        }

        private static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static void SendException(Client c, Exception e)
        {
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
                Console.WriteLine($"Getting {dir}");

                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        string root = dir.Remove(dir.LastIndexOf("\\"));
                        if (root.Length == 2)
                            root += "\\";

                        var directories = Directory.GetDirectories(dir);
                        var files = Directory.GetFiles(dir);

                        List<string[]> temp = new List<string[]>();
                        temp.Add(new string[]
                        {
                            "..",
                            root,
                            " ",
                            " ",
                            " "
                        });
                        foreach (var d in directories)
                        {
                            try 
                            {
                                DirectoryInfo di = new DirectoryInfo(d);
                                temp.Add(new string[]
                                {
                                    di.Name,
                                    di.FullName,
                                    di.GetFiles().Length + " FILES",
                                    di.CreationTime.ToShortDateString(),
                                    di.LastAccessTime.ToString("dd-mm-yy HH:ss")
                                });
                            }
                            catch (Exception e)
                            {
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
                                    fi.CreationTime.ToShortDateString(),
                                    fi.LastAccessTime.ToString("dd-mm-yy HH:ss"),
                                    fi.Length.ToString()
                                });
                            }
                            catch (Exception e)
                            {
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
    }
}
