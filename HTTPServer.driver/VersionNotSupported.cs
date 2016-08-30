using System.Text;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class VersionNotSupported : IHttpHandler
    {
        public Reply Execute(Request request)
        {
            var reply = new Reply();
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 505 HTTP Version Not Supported\r\n");
            return reply;
        }
    }
}
