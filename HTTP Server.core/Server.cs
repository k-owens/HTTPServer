using System;
using System.Net;
using System.Net.Sockets;

namespace HTTPServer.core
{
    public class Server
    {
        public bool Running = false;
        private Socket socket;
        private Socket client;

        public bool Start()
        {
            if (Running) return false;

            Running = true;
            Console.WriteLine("Echo server has started.");
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var ipEndPoint = new IPEndPoint(ipAddress, 8080);
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            PrepareSocketForConnection(ipEndPoint);
            return true;
        }

        private void PrepareSocketForConnection(IPEndPoint ipEndPoint)
        {
            socket.Bind(ipEndPoint);
            Console.WriteLine("Server socket is now bound to an end point.");
            socket.Listen(100);
            Console.WriteLine("Server has started listening.");
        }

        public void ConnectToClient()
        {
            client = socket.Accept();
            Console.WriteLine("Server has connected to the client.");
        }

        public void HandleData()
        {
            byte[] buffer = new byte[1024];

            ReceiveData(buffer);
            SendData(buffer);
            client.Close();
        }

        private void SendData(byte[] buffer)
        {
            client.Send(buffer);
            Console.WriteLine("Server is sending data back.");
        }

        private void ReceiveData(byte[] buffer)
        {
            client.Receive(buffer);
            Console.WriteLine("Server has received data.");
        }

        public bool Stop()
        {
            if (!Running) return false;

            Running = false;
            socket.Close();
            Console.WriteLine("Server has stopped.");
            return true;
        }
    }
}
