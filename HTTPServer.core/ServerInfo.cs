namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public IPathContents PathContents { get; set; }
        public IHttpHandler HttpHandler { get; }


        public ServerInfo(int port, IPathContents pathContents, IHttpHandler httpHandler)
        {
            HttpHandler = httpHandler;
            Port = port;
            PathContents = pathContents;
        }
    }
}
