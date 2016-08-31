using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class PutCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return request.Method.Equals("PUT") && request.HttpVersion.Equals("HTTP/1.1") && !request.Uri.Equals("/");
        }
    }
}
