namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public ISocket StartSocket { get; }
        public IDirectoryContents DirectoryContents { get; }

        public ServerInfo(int port, ISocket startSocket, IDirectoryContents directoryContents)
        {
            Port = port;
            StartSocket = startSocket;
            DirectoryContents = directoryContents;
        }
    }
}
