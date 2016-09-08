using System.Text;
using HTTPServer.core;
using System;
using System.IO;

namespace HTTPServer.app
{
    public class BadRequestFilter : IHttpHandler
    {
        private IPathContents _pathContents;
        private IHttpHandler _nextCommand;

        public BadRequestFilter(IPathContents pathContents, IHttpHandler nextCommand)
        {
            _pathContents = pathContents;
            _nextCommand = nextCommand;
        }

        public Reply Execute(Request request)
        {
            if (IsMalformed(request))
            {
                var reply = new Reply();
                reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\n");
                return reply;
            }
            var returnedReply = _nextCommand.Execute(request);
            LogData(LogMessage(request, returnedReply));
            return returnedReply;
        }
        
        private void LogData(string loggingMessage)
        {
            Console.Write(loggingMessage);
        }

        private string LogMessage(Request request, Reply reply)
        {
            var responseCode = GetResponseCode(reply.StartingLine);
            var logMessage = DateTime.Now.ToString() + " " + request.Method + " " + request.Uri + " "
                + request.HttpVersion + " " + responseCode + "\r\n";
            return logMessage;
        }

        public string GetResponseCode(byte[] response)
        {
            var firstLine = Encoding.UTF8.GetString(response);
            var requestLine = firstLine.Split(' ', ' ');
            return requestLine[1];
        }

        private bool IsMalformed(Request request)
        {
            return !IsValidMethod(request) || !request.HttpVersion.Substring(0, 5).Equals("HTTP/");
        }

        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST") || request.Method.Equals("PUT") || request.Method.Equals("DELETE");
        }
    }
}
