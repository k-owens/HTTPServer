using System;
using System.Net;
using Xunit;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class ServerUnitTests
    {
        [Fact]
        public void ServerCanStart()
        {
            var server = new Server();
            Assert.True(server.Start() && server.running == false);
        }
    }
}
