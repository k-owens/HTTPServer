using System;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class PostCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return request.Method.Equals("POST") && !request.Uri.Equals("/");
        }
    }
}
