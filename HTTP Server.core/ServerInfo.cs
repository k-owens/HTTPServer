namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public IPathContents PathContents { get; set; }
        public RequestRouter ServerRequestRouter { get; }


        public ServerInfo(int port, IPathContents pathContents, RequestRouter requestRouter)
        {
            ServerRequestRouter = requestRouter;
            Port = port;
            PathContents = pathContents;
        }
    }
}
