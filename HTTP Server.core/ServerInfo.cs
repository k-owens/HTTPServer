namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public IPathContents PathContents { get; set; }
        public RequestHandler ServerRequestHandler { get; }


        public ServerInfo(int port, IPathContents pathContents, RequestHandler requestHandler)
        {
            ServerRequestHandler = requestHandler;
            Port = port;
            PathContents = pathContents;
        }
    }
}
