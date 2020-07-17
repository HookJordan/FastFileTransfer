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
}
