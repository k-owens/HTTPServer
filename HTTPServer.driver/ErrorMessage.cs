using System.Text;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class ErrorMessage : IHttpHandler
    {
        private IPathContents _pathContents;

        public ErrorMessage(IPathContents pathContents)
        {
            _pathContents = pathContents;
        }

        public byte[] Execute(Request request)
        {
            if(!ShouldRun(request))
            {
                var getDirectoryContents = new GetDirectoryContents(_pathContents);
                return getDirectoryContents.Execute(request);
            }
            if (!IsValidMethod(request) || !request.HttpVersion.Substring(0, 5).Equals("HTTP/"))
                return Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\n");
            return Encoding.UTF8.GetBytes("HTTP/1.1 505 HTTP Version Not Supported\r\n");
        }

        private bool ShouldRun(Request request)
        {
            return !IsValidMethod(request) || !request.HttpVersion.Equals("HTTP/1.1");
        }
        
        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST");
        }
    }
}
