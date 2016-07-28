using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private byte[] bytesReturned;
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        private Socket socket;

        [TestMethod]
        public void ServerCanStart()
        {
            Assert.True(server.Start() && server.Running);
            server.Stop();
        }

        [TestMethod]
        public void ServerCanStop()
        {
            server.Start();
            Assert.True(server.Stop() && !server.Running);
        }

        [TestMethod]
        public void ServerCanEchoData()
        {
            SetUpClient();
            ConnectClientToServer(socket, ipEndPoint);
            var message = CommunicateWithServer(socket, bytesReturned);
            CloseConnectionWithServer(socket);
            Assert.Equal("This is the message sent by the client.", message);
        }

        private void SetUpClient()
        {
            bytesReturned = new byte[1024];
            ipAddress = IPAddress.Parse("127.0.0.1");
            ipEndPoint = new IPEndPoint(ipAddress, 8080);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void CloseConnectionWithServer(Socket socket)
        {
            server.Stop();
            socket.Close();
        }

        private string CommunicateWithServer(Socket socket, byte[] bytesReturned)
        {
            byte[] bytesToSend;
            bytesToSend = Encoding.UTF8.GetBytes("This is the message sent by the client.");
            socket.Send(bytesToSend);
            server.HandleData();
            socket.Receive(bytesReturned);
            var message = Encoding.UTF8.GetString(bytesReturned, 0, bytesToSend.Length);
            return message;
        }

        private void ConnectClientToServer(Socket socket, IPEndPoint ipEndPoint)
        {
            server.Start();
            socket.Connect(ipEndPoint);
            server.ConnectToClient();
        }
    }
}
