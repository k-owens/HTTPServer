namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public ISocket StartSocket { get; set; }
        public ISocket ClientConnection { get; set; }
        public IDirectoryContents DirectoryContents { get; set; }
        public IFileContents FileContents { get; set; }

        public ServerInfo(int port, ISocket startSocket, IDirectoryContents directoryContents, IFileContents fileContents)
        {
            Port = port;
            StartSocket = startSocket;
            DirectoryContents = directoryContents;
            FileContents = fileContents;
        }
    }
}
