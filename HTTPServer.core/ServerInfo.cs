namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public IPathContents PathContents { get; set; }
        public IHttpHandler HttpHandler { get; }
        public int Timeout { get; }


        public ServerInfo(int port, IPathContents pathContents, IHttpHandler httpHandler, int timeout)
        {
            HttpHandler = httpHandler;
            Port = port;
            PathContents = pathContents;
            Timeout = timeout;
        }
    }
}
