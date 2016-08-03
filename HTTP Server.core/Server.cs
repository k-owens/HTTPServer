using System;
using System.Net;
using System.Text;

namespace HTTPServer.core
{
    public class Server
    {
        public bool Running = false;
        private ISocket socket;
        private ISocket clientConnection;

        public Server Start(int port, ISocket startSocket)
        {
            if (Running) return null;

            Running = true;
            var ipAddress = IPAddress.Any;
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            socket = startSocket;
            PrepareSocketForConnection(ipEndPoint);
            Console.WriteLine("Server has started at port " + ((IPEndPoint)socket.LocalEndPoint()).Port);
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
            int bytesReceived = ReceiveData(buffer);
            SendData(HandleReply(buffer,bytesReceived));
            clientConnection.Close();
        }

        private  byte[] HandleReply(byte[] request, int requestSize)
        {
            var requestMessage = Encoding.UTF8.GetString(request).Substring(0,requestSize);
            var uri = requestMessage.Split(' ',' ')[1];
            if (uri.Equals("/"))
                return Encoding.UTF8.GetBytes("http/1.1 200 OK\r\n");
            
            return Encoding.UTF8.GetBytes("http/1.1 404 Not Found\r\n");
        }

        private void SendData(byte[] buffer)
        {
            clientConnection.Send(buffer);
        }

        private int ReceiveData(byte[] buffer)
        {
            return clientConnection.Receive(buffer);
        }

        public bool Stop()
        {
            Running = false;
            socket?.Close();
            clientConnection?.Close();
            return true;
        }
    }
}
