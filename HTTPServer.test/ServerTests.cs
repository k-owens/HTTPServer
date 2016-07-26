using System.IO;
using Xunit;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class ServerUnitTests
    {
        private Server server = new Server();
        private MockConnection mock = new MockConnection();

        [Fact]
        public void ServerCanStart()
        {
            Assert.True(server.Start() && server.Running);
        }

        [Fact]
        public void ServerCanStop()
        {
            server.Start();
            Assert.True(server.Stop() && !server.Running);
        }

        [Fact]
        public void ServerCanRespond200()
        {
            var i = server.RespondWith200(mock);
            var sentMessage = ReadMockStream();
            Assert.Equal("HTTP/1.1 200 OK\r\n"
                         + "Content-Type:  text/html\r\n"
                         + "\r\n"
                         + "<html><body><h1>Hello World</h1></body></html>", sentMessage);
        }

        [Fact]
        public void ServerCanRespond404()
        {
            var i = server.RespondWith404(mock);
            var sentMessage = ReadMockStream();
            Assert.Equal("HTTP/1.1 404 Not Found\r\n"
                         + "Content-Type:  text/html\r\n"
                         + "\r\n"
                         + "<html><body><h1>The requested page was not found.</h1></body></html>", sentMessage);
        }

        private string ReadMockStream()
        {
            mock.MStream.Position = 0;
            var sReader = new StreamReader(mock.MStream);
            return sReader.ReadToEnd();
        }
    }
}
