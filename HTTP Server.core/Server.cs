using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HTTP_Server.core;

namespace HTTPServer.core
{
    public class Server
    {
        public bool Running = false;

        public bool Start()
        {
            if (!Running)
            {
                Running = true;
                return true;
            }
            return false;
        }

        public bool Stop()
        {
            if (Running)
            {
                Running = false;
                return true;
            }
            return false;
        }

        public void RespondWith404(IConnection connection)
        {
            
        }

        public void RespondWith200(IConnection connection)
        {
            connection.GetReply("HTTP/1.1 200 OK\n"
                + "Content-Type:  text/html\n"
                + "\n"
                + "<html><body><h1>Hello World</h1></body></html>");
        }
    }
}
