using System;
using System.Net;
using System.Net.Sockets;

namespace HTTPServer.core
{
    public class Server
    {
        public bool Running = false;
        private Socket socket;
        private Socket clientConnection;

        public Server Start(int port)
        {
            if (Running) return null;

            Running = true;
            var ipAddress = IPAddress.Any;
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            PrepareSocketForConnection(ipEndPoint);
            Console.WriteLine("Echo server has started at port " + port.ToString());
            return this;
        }

        public void HandleClients()
        {
            while (true)
            {
                ConnectToClient();
                HandleData();
            }
        }

        private void PrepareSocketForConnection(IPEndPoint ipEndPoint)
        {
            socket.Bind(ipEndPoint);
            socket.Listen(100);
        }

        private void ConnectToClient()
        {
            clientConnection = socket.Accept();
        }

        private void HandleData()
        {
            byte[] buffer = new byte[1024];

            ReceiveData(buffer);
            SendData(buffer);
            clientConnection.Close();
        }

        private void SendData(byte[] buffer)
        {
            clientConnection.Send(buffer);
        }

        private void ReceiveData(byte[] buffer)
        {
            clientConnection.Receive(buffer);
        }

        public bool Stop()
        {
            if (!Running) return false;

            Running = false;
            socket.Close();
            return true;
        }
    }
}
