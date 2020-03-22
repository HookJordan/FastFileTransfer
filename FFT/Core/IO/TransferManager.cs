using FFT.Core.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.IO
{
    class TransferManager
    {
        public Dictionary<string, FileTransfer> FileTransfers = new Dictionary<string, FileTransfer>();

        public FileTransfer LocalDownload(Client client, string localPath, string remotePath, long length)
        {
            FileTransfer ft = new FileTransfer(localPath, remotePath, length, client.compressionProvider);
            FileTransfers.Add(ft.TransferId, ft);

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(ft.RemoteFilePath);
                    bw.Write(ft.LocalFilePath);
                    bw.Write(ft.TransferId);
                }

                client.Send(Packet.Create(PacketHeader.DownloadFile, ms.ToArray()));
            }

            return ft;
        }

        public FileTransfer LocalUpload(Client c, string localPath, string remotePath)
        {
            FileTransfer ft = new FileTransfer(localPath, remotePath, c.compressionProvider);
            FileTransfers.Add(ft.TransferId, ft);

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(remotePath);
                    bw.Write(localPath);
                    bw.Write(ft.FileLength.ToString());
                    bw.Write(ft.TransferId);
                }

                c.Send(Packet.Create(PacketHeader.UploadFile, ms.ToArray()));
            }

            return ft;
        }

        public FileTransfer CancelTransfer(string transferId)
        {
            if (FileTransfers.ContainsKey(transferId))
            {
                FileTransfer fileTransfer = FileTransfers[transferId];

                fileTransfer.Cancel();

                return fileTransfer;
            }

            throw new Exception($"Could not find file transfer with transfer id {transferId}");
        }

        public void StartUpload(Client c, Packet p)
        {
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string localPath = br.ReadString();
                    string remotePath = br.ReadString();

                    FileTransfer t = new FileTransfer(localPath, remotePath, c.compressionProvider);
                    t.TransferId = br.ReadString();
                    FileTransfers.Add(t.TransferId, t);

                    // START THE UPLOADING NOW 
                    NextChunk(c, t.TransferId, t.GetChunk());
                }
            }
        }

        public void StartDownload(Client c, Packet p)
        {
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string localPath = br.ReadString();
                    string remotePath = br.ReadString();
                    long len = long.Parse(br.ReadString());

                    FileTransfer t = new FileTransfer(localPath, remotePath, len, c.compressionProvider);
                    t.TransferId = br.ReadString();
                    FileTransfers.Add(t.TransferId, t);

                    NextChunk(c, t.TransferId, new byte[] { 0 });
                }
            }
        }

        public FileTransfer HandleFileChunk(Client c, Packet p)
        {
            using (MemoryStream ms = new MemoryStream(p.Payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string id = br.ReadString();

                    if (FileTransfers.ContainsKey(id))
                    {
                        var t = FileTransfers[id];

                        if (t.TransferType == TransferType.Upload)
                        {
                            NextChunk(c, id, t.GetChunk());
                        }
                        else
                        {
                            int sizeOfChunk = br.ReadInt32();
                            if (sizeOfChunk > 0)
                            {
                                byte[] chunk = br.ReadBytes(sizeOfChunk);
                                t.WriteChunk(chunk);
                            }
                            if (t.Transfering)
                            {
                                NextChunk(c, id, new byte[] { 0 });
                            }
                        }

                        return t;
                    }
                }
            }

            return null;
        }

        private void NextChunk(Client c, string id, byte[] chunk)
        {
            if (!FileTransfers[id].Paused)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(id);
                        bw.Write(chunk.Length);
                        bw.Write(chunk);
                    }
                    c.Send(Packet.Create(PacketHeader.FileChunk, ms.ToArray()));
                }
            }
        }
    }
}
