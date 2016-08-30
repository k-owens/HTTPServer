using System;
using System.Text;
using HTTPServer.core;

namespace HTTPServer.app
{
    public class GetDirectoryContents : IHttpHandler
    {
        private IPathContents _directoryContents;

        public GetDirectoryContents(IPathContents directoryContents)
        {
            _directoryContents = directoryContents;
        }

        public Reply Execute(Request request)
        {
            return ObtainDirectoryContents();
        }

        private Reply ObtainDirectoryContents()
        {
            var reply = new Reply();
            string bodyMessage;
            try
            {
                bodyMessage = GetBodyOfMessage(_directoryContents.GetDirectories(), _directoryContents.GetFiles());
            }
            catch
            {
                bodyMessage = "";
            }
            reply.Body = Encoding.UTF8.GetBytes(bodyMessage);
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n");
            reply.Headers = Encoding.UTF8.GetBytes("Content-Length: " + reply.Body.Length + "\r\n");
            return reply;
        }

        private static string GetBodyOfMessage(string[] directories, string[] files)
        {
            var bodyMessage = "<html>" + "<body>";
            foreach (var folder in directories)
            {
                bodyMessage += "<p>" + folder + "</p>";
            }
            foreach (var file in files)
            {
                bodyMessage += "<p>" + file + "</p>";
            }
            bodyMessage += "</body>" + "</html>";
            return bodyMessage;
        }
    }
}
