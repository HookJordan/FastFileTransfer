using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFT.Core.Compression;

namespace FFT.Core.IO
{
    public enum TransferType
    {
        Upload = 0,
        Download
    }

    class FileTransfer
    {
        public string LocalFilePath { get; private set; }
        public string RemoteFilePath { get; private set; }
        public string TransferId { get; set; }
        public long FileLength { get; private set; }
        public TransferType TransferType { get; private set; }
        public bool Paused { get; private set; }

        private CompressionProvider compressionProvider;

        private long LAST_TICK = 0;
        private long PER_SECOND_DELTA = 0;
        public long PER_SECOND { get; private set; } = 0;

        public bool Transfering 
        { 
            get
            {
                return FileLength > 0;
            } 
        }

        private FileStream fileStream;
        private byte[] buffer = new byte[65535];
        private double fullLength = 0;

        public FileTransfer(string local, string remote, CompressionProvider compressionProvider)
        {
            this.compressionProvider = compressionProvider;
            this.LocalFilePath = local;
            this.RemoteFilePath = remote;
            this.TransferType = TransferType.Upload;
            this.TransferId = Guid.NewGuid().ToString();

            fileStream = new FileStream(this.LocalFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            FileLength = fileStream.Length;
            this.fullLength = fileStream.Length;
        }

        public FileTransfer(string local, string remote, long length, CompressionProvider compressionProvider)
        {
            this.compressionProvider = compressionProvider;
            this.LocalFilePath = local;
            this.RemoteFilePath = remote;
            this.TransferType = TransferType.Download;
            this.TransferId = Guid.NewGuid().ToString();
            this.FileLength = length;
            this.fullLength = length;
            fileStream = new FileStream(this.LocalFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public void WriteChunk(byte[] chunk)
        {
            if (chunk.Length == 0 || fileStream == null) return;

            chunk = compressionProvider.Decompress(chunk); // Compression.Decompress(chunk);

            fileStream.Write(chunk, 0, chunk.Length);
            fileStream.Flush();

            FileLength -= chunk.Length;

            if (FileLength == 0)
            {
                Finish();
            }

            UpdateSpeed(chunk.Length);
        }

        public byte[] GetChunk()
        {
            if (fileStream == null) return new byte[] { };

            int read = fileStream.Read(buffer, 0, buffer.Length);
            
            if (read < buffer.Length)
            {
                Array.Resize(ref buffer, read);
            }

            FileLength -= read;
            if (FileLength == 0)
            {
                Finish();
            }

            UpdateSpeed(read);

            // long lenBeforeCompress = buffer.Length;
            // byte[] compress = Compression.Compress(buffer);
            // long lenAfterCompress = compress.Length;
            // Console.WriteLine($"Before {lenBeforeCompress} - After {lenAfterCompress}");

            return compressionProvider.Compress(buffer); // Compression.Compress(buffer);
        }

        private void UpdateSpeed(int amount)
        {
            long now = Environment.TickCount;

            if (now - LAST_TICK >= 1000)
            {
                PER_SECOND = PER_SECOND_DELTA;
                PER_SECOND_DELTA = amount;
                LAST_TICK = now;
            }
            else
            {
                PER_SECOND_DELTA += amount;
            }
        }

        public void Finish()
        {
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;
        }

        public void Cancel()
        {
            // Close off file stream
            Finish();

            // Delete file that was in progress 
            if (TransferType == TransferType.Download)
            {
                File.Delete(LocalFilePath);
            }
        }

        public int CalculatePct()
        {
            double max = fullLength;
            double current = (double)this.FileLength;

            return 100 - (int)((current / max) * 100.0);
        }

        public long Transferred()
        {
            return (long)this.fullLength - this.FileLength;
        }

        public void TogglePause()
        {
            this.Paused = !this.Paused;
        }
    }
}
