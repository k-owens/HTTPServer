using System;
using System.Net;
using Xunit;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class ServerUnitTests
    {
        Server server = new Server();

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
            FileConnection file = new FileConnection();

            server.Start();
            server.RespondWith200(file);
            String str = System.IO.File.ReadAllText(
                @"C:\gitwork\HTTP Server\HTTPServer.test\ReplyOutput.txt");
            Assert.Equal("HTTP/1.1 200 OK\n"
                + "Content-Type:  text/html\n"
                + "\n"
                + "<html><body><h1>Hello World</h1></body></html>", str);
        }
    }
}
