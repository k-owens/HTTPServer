using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer.core
{
    public class Server
    {
        private Socket _socket;
        private Socket _clientConnection;
        private IPathContents _pathContents;
        private IHttpHandler _httpHandler;
        private static bool keepRunning = true;
        private int _timeout;

        public Server Start(ServerInfo serverInfo)
        {
            SetupSocket(serverInfo);
            _pathContents = serverInfo.PathContents;
            _httpHandler = serverInfo.HttpHandler;
            _timeout = serverInfo.Timeout;
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
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                keepRunning = false;
                if (_timeout != 0)
                {
                    System.Threading.Thread.Sleep(_timeout);
                    Environment.Exit(0);
                }
            };

            while (keepRunning)
            {
                _clientConnection = _socket.Accept();
                RespondToClient();
                _clientConnection.Close();
            }
        }

        private void RespondToClient()
        {
            byte[] clientMessage = Read();
            Request request= new Request(clientMessage);
            byte[] reply = GetReply(request).ReplyMessage();
            _clientConnection.Send(reply);
        }

        private Reply GetReply(Request request)
        {
            return _httpHandler.Execute(request);
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
                ReadKbOfData(messageReceived);
                if (IsDoneReading(messageReceived))
                    break;
            }
            return messageReceived;
        }

        private bool IsDoneReading(List<byte> messageReceived)
        {
            var messageBytes = FormatObtainedData(messageReceived);
            var message = Encoding.UTF8.GetString(messageBytes);
            return message.Contains("\r\n\r\n") && HasWholeBody(message);
        }

        private bool HasWholeBody(string message)
        {
            if(!message.Contains("Content-Length:"))
                return true;
            var headers = message.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var contentLength = CalculateContentLength(headers);
            var headerBodySplit = message.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            return contentLength <= headerBodySplit[1].Length;
        }

        private int CalculateContentLength(string[] headers)
        {
            for (int i = 0; i < headers.Length - 1; i++)
            {
                if (headers[i].Contains("Content-Length:"))
                {
                    var startOfNumber = headers[i].IndexOf("Content-Length:") + 16;
                    return Int32.Parse(headers[i].Substring(startOfNumber, headers[i].Length - startOfNumber));
                }
            }
            return 0;
        }

        private void ReadKbOfData(List<byte> messageReceived)
        {
            var buffer = new byte[1024];
            var bytesReceived = _clientConnection.Receive(buffer);
            for (var bufferIndex = 0; bufferIndex < bytesReceived; bufferIndex++)
                messageReceived.Add(buffer[bufferIndex]);
        }

        public bool Stop()
        {
            _socket?.Close();
            _clientConnection?.Close();
            return true;
        }
    }
}
