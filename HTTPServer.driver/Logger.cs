using System;
using HTTPServer.Core;
using System.Text;

namespace HTTPServer.app
{
    public class Logger
    {
        public void LogData(string loggingMessage)
        {
            Console.Write(loggingMessage);
        }

        public string LogMessage(Request request, Reply reply)
        {
            var responseCode = GetResponseCode(reply.StartingLine);
            var logMessage = DateTime.Now.ToString() + " " + request.Method + " " + request.Uri + " "
                + request.HttpVersion + " " + responseCode + "\r\n";
            return logMessage;
        }

        private string GetResponseCode(byte[] response)
        {
            var firstLine = Encoding.UTF8.GetString(response);
            var requestLine = firstLine.Split(' ', ' ');
            return requestLine[1];
        }
    }
}
