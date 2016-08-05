using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using HTTPServer.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace HTTPServer.test
{
    [TestClass]
    public class ServerUnitTests
    {
        private Server server = new Server();
        private byte[] _bytesReturned;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;
        private Socket _socket;

        [TestMethod]
        public void ServerCanStart()
        {
            Assert.True(server.Start(8080, new NetworkSocket(), "") != null && server.Running);
            server.Stop();
        }

        [TestMethod]
        public void ServerCanStop()
        {
            server.Start(8080, new NetworkSocket(), "");
            Assert.True(server.Stop() && !server.Running);
        }

        [TestMethod]
        public void ServerCanReturn200()
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
        public void ServerCanReturn404()
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
                                               "\r\n" +
                                               "<html>\r\n" +
                                               "<body>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\.git\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\.vs\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTP Server.core\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTPServer.driver\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTPServer.run\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTPServer.test\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\packages\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\TestResults\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\.gitattributes\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\.gitignore\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTP Server.core.dll\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\HTTPServer.sln\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\server.config\r\n" +
                                               "</p>\r\n" +
                                               "<p>\r\n" +
                                               "C:\\gitwork\\HTTP Server\\server.exe\r\n" +
                                               "</p>\r\n" +
                                               "</body>\r\n" +
                                               "</html>", @"C:\gitwork\HTTP Server");
        }

        private static void TestResponse(string request, string expectedReply, string directory)
        {
            MockConnection mock = new MockConnection();
            MockConnection serverConnection = new MockConnection();
            Server testServer = new Server();
            byte[] buffer = new byte[1024];

            mock.Send(Encoding.UTF8.GetBytes(request));
            testServer.Start(0, serverConnection, directory);
            testServer.ConnectToClient();
            testServer.HandleData();
            int bytesReceived = serverConnection.Receive(buffer);
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
            server.Stop();
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
            server.Start(8080, new NetworkSocket(), "");
            socket.Connect(ipEndPoint);
            server.HandleClients();
        }
    }
}
