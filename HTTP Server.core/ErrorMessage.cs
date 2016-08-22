using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPServer.core;

namespace HTTP_Server.core
{
    public class ErrorMessage : IHttpHandler
    {
        public byte[] Execute(Request request)
        {
            if (!IsValidMethod(request) || !request.HttpVersion.Substring(0, 5).Equals("HTTP/"))
                return Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\n");
            return Encoding.UTF8.GetBytes("HTTP/1.1 505 HTTP Version Not Supported\r\n");
        }

        public bool ShouldRun(Request request, IPathContents pathContents)
        {
            return !IsValidMethod(request) || !request.HttpVersion.Equals("HTTP/1.1\r\n");
        }
        
        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST");
        }
    }
}
