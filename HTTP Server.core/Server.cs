using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace HTTPServer.core
{
    public class Server
    {
        private Socket _socket;
        private Socket _clientConnection;
        private IPathContents _pathContents;
        private RequestRouter _requestRouter;

        public Server Start(ServerInfo serverInfo)
        {
            SetupSocket(serverInfo);
            _pathContents = serverInfo.PathContents;
            _requestRouter = serverInfo.ServerRequestRouter;
            Console.WriteLine("Server has started at port " + ((IPEndPoint)_socket.LocalEndPoint).Port);
            return this;
        }

        private void SetupSocket(ServerInfo serverInfo)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipEndPoint = new IPEndPoint(IPAddress.Any, serverInfo.Port);
            PrepareSocketForConnection(ipEndPoint);
        }

        private void PrepareSocketForConnection(IPEndPoint ipEndPoint)
        {
            _socket.Bind(ipEndPoint);
            _socket.Listen(100);
        }

        public void HandleClients()
        {
            while (true)
            {
                _clientConnection = _socket.Accept();
                RespondToClient();
                _clientConnection.Close();
            }
        }

        private void RespondToClient()
        {
            string ipAddress = ((IPEndPoint)_clientConnection.RemoteEndPoint).Address.ToString();
            byte[] clientMessage = Read();
            Request request= new Request(clientMessage);
            byte[] reply = GetReply(request);
            var loggedMessage = request.LogRequest(reply, ipAddress, DateTime.Now);
            System.IO.File.AppendAllText("../logs.txt", loggedMessage);
            _clientConnection.Send(reply);
        }

        private byte[] GetReply(Request request)
        {
            return _requestRouter.HandleData(request, _pathContents);
        }


        private byte[] Read()
        {
            var messageReceived = PullDataFromClient();
            return FormatObtainedData(messageReceived);
        }

        private byte[] FormatObtainedData(List<byte> messageReceived)
        {
            var message = new byte[messageReceived.Count];
            for (var copyIndex = 0; copyIndex < messageReceived.Count; copyIndex++)
                message[copyIndex] = messageReceived[copyIndex];
            return message;
        }

        private List<byte> PullDataFromClient()
        {
            var messageReceived = new List<byte>();
            while (true)
            {
                var bytesReceived = ReadKbOfData(messageReceived);
                if (IsLessThanKb(bytesReceived))
                    break;
            }
            return messageReceived;
        }

        private static bool IsLessThanKb(int bytesReceived)
        {
            return bytesReceived < 1024;
        }

        private int ReadKbOfData(List<byte> messageReceived)
        {
            var buffer = new byte[1024];
            var bytesReceived = ReceiveData(buffer);
            for (var bufferIndex = 0; bufferIndex < bytesReceived; bufferIndex++)
                messageReceived.Add(buffer[bufferIndex]);
            return bytesReceived;
        }

        private int ReceiveData(byte[] buffer)
        {
            return _clientConnection.Receive(buffer);
        }

        public bool Stop()
        {
            _socket?.Close();
            _clientConnection?.Close();
            return true;
        }
    }
}
