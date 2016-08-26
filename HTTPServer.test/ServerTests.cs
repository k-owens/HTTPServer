using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HTTPServer.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;
using HTTPServer.app;

namespace HTTPServer.test
{
    [TestClass]
    public class ServerUnitTests
    {
        private byte[] _bytesReturned;
        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;

        [TestMethod]
        public void ServerCanStart()
        {
            Server server = new Server();
            RequestRouter requestRouter = AddFunctionality();
            ServerInfo info = new ServerInfo(8080, new MockPathContents(""), requestRouter);
            Assert.True(server.Start(info) != null);
            server.Stop();
        }

        [TestMethod]
        public void ServerCanStop()
        {
            Server server = new Server();
            RequestRouter requestRouter = AddFunctionality();
            ServerInfo info = new ServerInfo(8080, new MockPathContents(""), requestRouter);
            server.Start(info);
            Assert.True(server.Stop());
        }

        [TestMethod]
        public void ServerCanReturn200Integration()
        {
            var message = IntegrationRun("GET / HTTP/1.1\r\n");
            Assert.Equal("HTTP/1.1 200 OK\r\n" +
                                               "Content-Length: 98\r\n" +
                                               "\r\n" +
                                               "<html>" +
                                               "<body>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\.git" +
                                               "</p>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\file.txt" +
                                               "</p>" +
                                               "</body>" +
                                               "</html>", message);
        }

        private string IntegrationRun(string sentMessage)
        {
            Socket socket = SetUpClient();
            Server server = new Server();
            var message = "";
            Action<object> action1 = (object obj) => { ConnectClientToServer(socket, _ipEndPoint, server); };

            Action<object> action2 =
                (object obj) => { message = CommunicateWithServer(socket, _bytesReturned, sentMessage); };

            Task t1 = new Task(action1, "");
            Task t2 = new Task(action2, "");

            t1.Start();
            System.Threading.Thread.Sleep(100);
            t2.Start();
            t2.Wait();
            CloseConnectionWithServer(socket,server);
            return message;
        }

        [TestMethod]
        public void ServerCanReturn404Integration()
        {
            var message = IntegrationRun("GET /extension HTTP/1.1\r\n");
            Assert.Equal("HTTP/1.1 404 Not Found\r\n", message);
        }

        [TestMethod]
        public void ServerCanReply404()
        {
            TestResponse("GET /extension HTTP/1.1\r\n","HTTP/1.1 404 Not Found\r\n","");
        }

        [TestMethod]
        public void ServerWillGive400ForMalformedRequests()
        {
            var message = IntegrationRun("GET / http/1.1\r\n");
            Assert.Equal("HTTP/1.1 400 Bad Request\r\n", message);
        }

        [TestMethod]
        public void ServerCanReturnFilesInDirectory()
        {
            TestResponse("GET / HTTP/1.1\r\n", "HTTP/1.1 200 OK\r\n" +
                                               "Content-Length: 98\r\n" +
                                               "\r\n" +
                                               "<html>" +
                                               "<body>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\.git" +
                                               "</p>" +
                                               "<p>" +
                                               "C:\\gitwork\\HTTP Server\\file.txt" +
                                               "</p>" +
                                               "</body>" +
                                               "</html>", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void ServerCanReplyWithFileContents()
        {
            TestResponse("GET /file.txt HTTP/1.1\r\n", "HTTP/1.1 200 OK\r\n" +
                                               "Content-Length: 32\r\n" +
                                               "\r\n" +
                                               "This is the content of the file.", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void ServerWillGive505ForBadVersion()
        {
            var message = IntegrationRun("GET / HTTP/1.0\r\n");
            Assert.Equal("HTTP/1.1 505 HTTP Version Not Supported\r\n", message);
        }

        [TestMethod]
        public void ServerWillRespondToPost()
        {
            TestResponse("POST /fileExample.txt HTTP/1.1\r\n\r\nThis will be in the file.", "HTTP/1.1 201 Created\r\n", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void CanGetPartialContents()
        {
            TestResponse("GET /file.txt HTTP/1.1\r\n" +
                         "Range: bytes=0-10\r\n\r\n", "HTTP/1.1 206 Partial Content\r\n" +
                                               "Content-Length: 11\r\n" +
                                               "Content-Range: bytes 0-10\r\n" +
                                               "\r\n" +
                                               "This is the", @"C:\gitwork\HTTP Server");
        }

        [TestMethod]
        public void RequestsCanBeLogged()
        {
                var requestMessage = Encoding.UTF8.GetBytes("GET /extension.txt HTTP/1.1\r\n");
                Request request = new Request(requestMessage);
                var response = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n");
                var logMessage = request.LogRequest(response, "127.0.0.1",DateTime.Today);
                Assert.Equal("127.0.0.1 " + DateTime.Today + " GET /extension.txt HTTP/1.1 404\r\n", logMessage);
        }

        private static RequestRouter AddFunctionality()
        {
            MockPathContents pathContents = new MockPathContents(@"C:\gitwork\HTTP Server");
            RequestRouter requestRouter = new RequestRouter(new ErrorMessage(pathContents));
            return requestRouter;
        }

        private static void TestResponse(string requestMessage, string expectedReply, string directory)
        {
            var requestHandler = AddFunctionality();
            var byteRequestMessage = Encoding.UTF8.GetBytes(requestMessage);
            var request = new Request(byteRequestMessage);
            var replyMessage = requestHandler.HandleData(request, new MockPathContents(directory));
            Assert.Equal(expectedReply, Encoding.UTF8.GetString(replyMessage));
        }

        private Socket SetUpClient()
        {
            _bytesReturned = new byte[1024];
            _ipAddress = IPAddress.Parse("127.0.0.1");
            _ipEndPoint = new IPEndPoint(_ipAddress, 8080);
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void CloseConnectionWithServer(Socket socket, Server server)
        {
            server.Stop();
            socket.Close();
        }

        private string CommunicateWithServer(Socket socket, byte[] bytesReturned, string incomingMessage)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(incomingMessage);
            socket.Send(bytesToSend);
            var bytesReceived = socket.Receive(bytesReturned);
            var message = Encoding.UTF8.GetString(bytesReturned).Substring(0, bytesReceived);
            return message;
        }

        private void ConnectClientToServer(Socket socket, IPEndPoint ipEndPoint, Server server)
        {
            RequestRouter requestRouter = AddFunctionality();
            ServerInfo info = new ServerInfo(8080, new MockPathContents(""), requestRouter);
            server.Start(info);
            socket.Connect(ipEndPoint);
            server.HandleClients();
        }
    }
}
