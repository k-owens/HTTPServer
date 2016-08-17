namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public ISocket StartSocket { get; set; }
        public ISocket ClientConnection { get; set; }
        public IPathContents PathContents { get; set; }

        public ServerInfo(int port, ISocket startSocket, IPathContents _pathContents)
        {
            Port = port;
            StartSocket = startSocket;
            PathContents = _pathContents;
        }
    }
}
