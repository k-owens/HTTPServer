using System.Text;
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

        public int RespondWith404(IConnection connection)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n"
                         + "Content-Type:  text/html\r\n"
                         + "\r\n"
                         + "<html><body><h1>The requested page was not found.</h1></body></html>");
            return connection.SendReply(buffer);
        }

        public int RespondWith200(IConnection connection)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n"
                         + "Content-Type:  text/html\r\n"
                         + "\r\n"
                         + "<html><body><h1>Hello World</h1></body></html>");
            return connection.SendReply(buffer);
        }
    }
}
