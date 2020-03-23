using System.IO;
using System.Text;

namespace FFT.Core.Networking
{
    public enum PacketHeader
    {
        Info = 1,
        PingPong,
        GetDrives,
        DrivesResponse,
        GetDirectory,
        DirectoryResponse,
        DownloadFile,
        UploadFile,
        FileChunk,
        CancelTransfer,
        DirectoryDelete,
        DirectoryCreate,
        DirectoryMove,
        FileDelete,
        FileMove,
        FileBrowserException,
        GoodBye
    }
    public class Packet
    {
        public PacketHeader PacketHeader { get; set; }
        public byte[] Payload { get; set; }

        public override string ToString() => Encoding.ASCII.GetString(Payload);

        // Hide constructor
        private Packet() { }

        public byte[] ToRawBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((int)this.PacketHeader);
                    bw.Write(this.Payload.Length);
                    bw.Write(this.Payload);
                }

                return ms.ToArray();
            }
        }

        public static Packet FromPayload(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream(payload))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Packet p = new Packet();
                    p.PacketHeader = (PacketHeader)br.ReadInt32();
                    p.Payload = br.ReadBytes(br.ReadInt32());

                    return p;
                }
            }
        }

        public static Packet Create(PacketHeader header, byte[] payload)
        {
            return new Packet() { PacketHeader = header, Payload = payload };
        }

        public static Packet Create(PacketHeader header, string payload)
        {
            return Create(header, Encoding.ASCII.GetBytes(payload));
        }
    }
}
