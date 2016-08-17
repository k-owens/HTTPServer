using System;
using System.Net;

namespace HTTPServer.core
{
    public class Server
    {
        private ISocket socket;
        private ISocket clientConnection;
        private IPathContents _pathContents;

        public Server Start(ServerInfo serverInfo)
        {
            socket = SocketConnector.SetupSocket(serverInfo);
            _pathContents = serverInfo.PathContents;
            Console.WriteLine("Server has started at port " + ((IPEndPoint)socket.LocalEndPoint()).Port);
            return this;
        }

        public void HandleClients()
        {
            while (true)
            {
                ConnectClient();
                RequestHandler.HandleData(clientConnection, _pathContents);
                clientConnection.Close();
            }
        }

        public void ConnectClient()
        {
            clientConnection = ClientConnector.ConnectToClient(socket);
        }

        public bool Stop()
        {
            socket?.Close();
            clientConnection?.Close();
            return true;
        }
    }
}
