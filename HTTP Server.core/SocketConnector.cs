using System.Net;

namespace HTTPServer.core
{
    public static class SocketConnector
    {

        public static ISocket SetupSocket(ServerInfo serverInfo)
        {
            var ipAddress = IPAddress.Any;
            var ipEndPoint = new IPEndPoint(ipAddress, serverInfo.Port);
            var socket = serverInfo.StartSocket;
            PrepareSocketForConnection(ipEndPoint, socket);
            return socket;
        }

        private static void PrepareSocketForConnection(IPEndPoint ipEndPoint, ISocket socket)
        {
            socket.Bind(ipEndPoint);
            socket.Listen(100);
        }
    }
}
