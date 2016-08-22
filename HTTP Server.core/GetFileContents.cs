using System;
using System.Text;

namespace HTTPServer.core
{
    public class GetFileContents : IHttpHandler
    {
        private IPathContents _fileContents;

        public GetFileContents(IPathContents fileContents)
        {
            _fileContents = fileContents;
        }

        public byte[] Execute(Request request)
        { 
            if (IsValidFile(request.Uri, _fileContents))           
                return ObtainFileContents(request.Uri);
            return Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n");
        }

        public bool ShouldRun(Request request, IPathContents pathContents)
        {
            return IsGetMethod(request.Method);
        }

        private bool IsValidFile(string uri, IPathContents pathContents)
        {
            try
            {
                var files = pathContents.GetFiles();
                foreach (string file in files)
                {
                    var expectedFilePath = GetExpectedFilePath(uri, pathContents);
                    if (expectedFilePath.Equals(file))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string GetExpectedFilePath(string uri, IPathContents pathContents)
        {
            var expectedFilePath = pathContents.DirectoryPath;
            expectedFilePath += "\\";
            expectedFilePath += uri.Substring(1);
            return expectedFilePath;
        }

        private bool IsGetMethod(string requestMethod)
        {
            return requestMethod.Equals("GET");
        }

        private byte[] ObtainFileContents(string uri)
        {
            var bodyMessage = _fileContents.GetFileContents(FormattedFilePath(_fileContents, uri));
            var messageHeaders = "HTTP/1.1 200 OK\r\n" + "Content-Length: " + bodyMessage.Length + "\r\n\r\n";
            var messageBytes = Encoding.UTF8.GetBytes(messageHeaders);
            var combinedMessage = new byte[messageBytes.Length + bodyMessage.Length];
            CombineArrays(messageBytes, combinedMessage, bodyMessage);
            return combinedMessage;
        }

        private static void CombineArrays(byte[] messageBytes, byte[] combinedMessage, byte[] bodyMessage)
        {
            Buffer.BlockCopy(messageBytes, 0, combinedMessage, 0, messageBytes.Length);
            Buffer.BlockCopy(bodyMessage, 0, combinedMessage, messageBytes.Length, bodyMessage.Length);
        }

        private static string FormattedFilePath(IPathContents pathContents, string uri)
        {
            var actualFilePath = pathContents.DirectoryPath;
            actualFilePath += "\\" + uri.Substring(1);
            return actualFilePath;
        }
    }
}
