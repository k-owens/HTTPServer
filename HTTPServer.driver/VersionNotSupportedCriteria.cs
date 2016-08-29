using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class VersionNotSupportedCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return IsValidMethod(request) && request.HttpVersion.Substring(0, 5).Equals("HTTP/") && !request.HttpVersion.Equals("HTTP/1.1");
        }
        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST");
        }
    }
}
