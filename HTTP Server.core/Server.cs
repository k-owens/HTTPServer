using System;
using System.Collections.Generic;
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

        public void ConnectToClient()
        {
            clientConnection = socket.Accept();
        }

        public void HandleData()
        {
            List<byte> messageReceived = new List<byte>();
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesReceived = ReceiveData(buffer);
                for (int i = 0; i < bytesReceived; i++)
                {
                    messageReceived.Add(buffer[i]);
                }
                if (bytesReceived < 1024)
                    break;
            }

            byte[] message = new byte[messageReceived.Count];
            for (int j = 0; j < messageReceived.Count; j++)
            {
                message[j] = messageReceived[j];
            }

            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(message));
            SendData(HandleReply(message, messageReceived.Count));
            clientConnection.Close();
        }

        private  byte[] HandleReply(byte[] request, int requestSize)
        {
            var requestMessage = Encoding.UTF8.GetString(request).Substring(0,requestSize);
            var uri = "";
            try
            {
                uri = requestMessage.Split(' ', ' ')[1];
            }
            catch
            {
                uri = "";
            }
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
