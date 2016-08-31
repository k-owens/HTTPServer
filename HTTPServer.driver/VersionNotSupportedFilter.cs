using System.Text;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class VersionNotSupportedFilter : IHttpHandler
    {
        private IHttpHandler _nextCommand;

        public VersionNotSupportedFilter(IHttpHandler nextCommand)
        {
            _nextCommand = nextCommand;
        }

        public Reply Execute(Request request)
        {
            if (IsInvalidVersion(request))
            {
                var reply = new Reply();
                reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 505 HTTP Version Not Supported\r\n");
                return reply;
            }
            return _nextCommand.Execute(request);
        }

        private bool IsInvalidVersion(Request request)
        {
            return IsValidMethod(request) && request.HttpVersion.Substring(0, 5).Equals("HTTP/") && !request.HttpVersion.Equals("HTTP/1.1");
        }

        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST");
        }
    }
}