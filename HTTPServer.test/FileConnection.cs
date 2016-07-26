using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTP_Server.core;

namespace HTTPServer.test
{
    public class FileConnection : IConnection
    {
        public void Request()
        {
            
        }

        public void GetReply(String message)
        {
            System.IO.File.WriteAllText(
                @"C:\gitwork\HTTP Server\HTTPServer.test\ReplyOutput.txt", message);
        }
    }
}
