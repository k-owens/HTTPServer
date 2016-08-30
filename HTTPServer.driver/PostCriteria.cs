using System;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class PostCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return request.Method.Equals("POST") && request.HttpVersion.Equals("HTTP/1.1\r\n") && !request.Uri.Equals("/");
        }
    }
}
