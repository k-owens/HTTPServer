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

        public byte[] Execute(Request request)
        {
            if(!ShouldRun(request))
            {
                var getFileContents = new GetFileContents(_directoryContents);
                return getFileContents.Execute(request);
            }
            return ObtainDirectoryContents();
        }

        private bool ShouldRun(Request request)
        {
            return IsRoot(request.Uri) && IsGetMethod(request.Method);
        }

        private bool IsGetMethod(string requestMethod)
        {
            return requestMethod.Equals("GET");
        }

        private bool IsRoot(string uri)
        {
            return uri.Equals("/");
        }

        private byte[] ObtainDirectoryContents()
        {
            string bodyMessage;
            try
            {
                bodyMessage = GetBodyOfMessage(_directoryContents.GetDirectories(), _directoryContents.GetFiles());
            }
            catch
            {
                bodyMessage = "";
            }
            var wholeMessage = "HTTP/1.1 200 OK\r\n" + "Content-Length: " + 
                bodyMessage.Length + "\r\n\r\n" + bodyMessage;
            return Encoding.UTF8.GetBytes(wholeMessage);
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
