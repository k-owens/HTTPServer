using System.Net.Sockets;

namespace HTTPServer.core
{
    public class ServerInfo
    {
        public int Port { get; }
        public IPathContents PathContents { get; set; }

        public ServerInfo(int port, IPathContents pathContents)
        {
            Port = port;
            PathContents = pathContents;
        }
    }
}
