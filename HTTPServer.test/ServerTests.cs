using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HTTPServer.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace HTTPServer.test
{
    [TestClass]
    public class ServerUnitTests
    {
        private readonly Server _server = new Server();
        private byte[] _bytesReturned;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;
        private Socket _socket;

        [TestMethod]
        public void ServerCanStart()
        {
            ServerInfo info = new ServerInfo(8080, new NetworkSocket(), new MockDirectoryContents(""), new MockFileContents());
            Assert.True(_server.Start(info) != null);
            _server.Stop();
        }

        [TestMethod]
        public void ServerCanStop()
        {
            ServerInfo info = new ServerInfo(8080, new NetworkSocket(), new MockDirectoryContents(""), new MockFileContents());
            _server.Start(info);
            Assert.True(_server.Stop());
        }

        [TestMethod]
        public void ServerCanReturn200Integration()
        {
            SetUpClient();
            var message = "";
            Action<object> action1 = (object obj) =>
            {
                ConnectClientToServer(_socket, _ipEndPoint);
            };

            Action<object> action2 = (object obj) =>
            {
                message = CommunicateWithServer200(_socket, _bytesReturned);
            };

            Task t1 = new Task(action1, "");
            Task t2 = new Task(action2, "");

            t1.Start();
            System.Threading.Thread.Sleep(100);
            t2.Start();
            t2.Wait();
            CloseConnectionWithServer(_socket);
            Assert.Equal("HTTP/1.1 200 OK\r\n", message);
        }

        [TestMethod]
        public void ServerCanReturn404Integration()
        {
            SetUpClient();
            var message = "";
            Action<object> action1 = (object obj) =>
            {
                ConnectClientToServer(_socket, _ipEndPoint);
            };

            Action<object> action2 = (object obj) =>
            {
                message = CommunicateWithServer404(_socket, _bytesReturned);
            };

            Task t1 = new Task(action1, "");
            Task t2 = new Task(action2, "");

            t1.Start();
            System.Threading.Thread.Sleep(100);
            t2.Start();
            t2.Wait();
            CloseConnectionWithServer(_socket);
            Assert.Equal("HTTP/1.1 404 Not Found\r\n", message);
        }

        [TestMethod]
        public void ServerCanReply200()
        {
            TestResponse("GET / HTTP/1.1\r\n", "HTTP/1.1 200 OK\r\n","");
        }

        [TestMethod]
        public void ServerCanReply404()
        {
            TestResponse("GET /extension HTTP/1.1\r\n","HTTP/1.1 404 Not Found\r\n","");
        }

        [TestMethod]
        public void ServerWillGive400ForMalformedRequests()
        {
            TestResponse("GET / http/1.1\r\n", "HTTP/1.1 400 Bad Request\r\n","");
        }

        [TestMethod]
        public void ServerWillGive400ForWrongVersion()
        {
            TestResponse("GET / HTTP/1.0", "HTTP/1.1 400 Bad Request\r\n","");
        }

        [TestMethod]
        public void ServerCanReturnFilesInDirectory()
        {
            TestResponse("GET / HTTP/1.1\r\n", "HTTP/1.1 200 OK\r\n" +
                                               "Content-Length: 104\r\n" +
                                               "\r\n" +
                                               "<html>" +
                                               "<body>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\.git" +
                                               "</p>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\.gitattributes" +
                                               "</p>" +
                                               "</body>" +
                                               "</html>", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void ServerCanReplyWithFileContents()
        {
            TestResponse("GET /.gitattributes HTTP/1.1\r\n", "HTTP/1.1 200 OK\r\n" +
                                               "Content-Length: 32\r\n" +
                                               "\r\n" +
                                               "This is the content of the file.", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void ServerWillGive505ForBadVersion()
        {
            TestResponse("GET / HTTP/1.0\r\n","HTTP/1.1 505 HTTP Version Not Supported\r\n","");
        }

        private static void TestResponse(string request, string expectedReply, string directory)
        {
            var mock = new MockConnection();
            var serverConnection = new MockConnection();
            var testServer = new Server();
            byte[] buffer = new byte[1024];

            mock.Send(Encoding.UTF8.GetBytes(request));
            var info = new ServerInfo(0, serverConnection, new MockDirectoryContents(directory), new MockFileContents());
            testServer.Start(info);
            testServer.ConnectClient();
            RequestHandler.HandleData(mock, new MockDirectoryContents(directory), new MockFileContents());
            var bytesReceived = serverConnection.Receive(buffer);
            Assert.Equal(expectedReply, Encoding.UTF8.GetString(buffer).Substring(0, bytesReceived));
        }

        private void SetUpClient()
        {
            _bytesReturned = new byte[1024];
            _ipAddress = IPAddress.Parse("127.0.0.1");
            _ipEndPoint = new IPEndPoint(_ipAddress, 8080);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void CloseConnectionWithServer(Socket socket)
        {
            _server.Stop();
            socket.Close();
        }

        private string CommunicateWithServer200(Socket socket, byte[] bytesReturned)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes("GET / HTTP/1.1\r\n");
            socket.Send(bytesToSend);
            var bytesReceived = socket.Receive(bytesReturned);
            var message = Encoding.UTF8.GetString(bytesReturned).Substring(0,bytesReceived);
            return message;
        }

        private string CommunicateWithServer404(Socket socket, byte[] bytesReturned)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes("GET /extension HTTP/1.1\r\n");
            socket.Send(bytesToSend);
            var bytesReceived = socket.Receive(bytesReturned);
            var message = Encoding.UTF8.GetString(bytesReturned).Substring(0, bytesReceived);
            return message;
        }

        private void ConnectClientToServer(Socket socket, IPEndPoint ipEndPoint)
        {
            ServerInfo info = new ServerInfo(8080, new NetworkSocket(), new MockDirectoryContents(""), new MockFileContents());
            _server.Start(info);
            socket.Connect(ipEndPoint);
            _server.HandleClients();
        }
    }
}
