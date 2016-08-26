using System;
using System.Text;
using System.Linq;
using HTTPServer.core;

namespace HTTPServer.app
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
            if(!ShouldRun(request))
            {
                var postContents = new PostContents(_fileContents);
                return postContents.Execute(request);
            }
            if (request.Uri.Equals("/logs"))
                return GetLogContents();
            if (IsValidFile(request.Uri, _fileContents))
                return ObtainFileContents(request);
            return Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n");
        }

        private byte[] GetLogContents()
        {
            var bodyMessage = _fileContents.GetFileContents("../logs.txt");
            var messageHeaders = "HTTP/1.1 200 OK\r\n" + "Content-Length: " + bodyMessage.Length + "\r\n\r\n";
            var headerBytes = Encoding.UTF8.GetBytes(messageHeaders);
            var combinedMessage = new byte[headerBytes.Length + bodyMessage.Length];
            CombineArrays(headerBytes, combinedMessage, bodyMessage);
            return combinedMessage;
        }

        private bool ShouldRun(Request request)
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

        private byte[] ObtainFileContents(Request request)
        {
            if (HasRangeHeader(request))
                return GetRangeResponse(request);
            return GetNormalResponse(request);
        }

        private byte[] GetNormalResponse(Request request)
        {
            var bodyMessage = _fileContents.GetFileContents(FormattedFilePath(_fileContents, request.Uri));
            var messageHeaders = "HTTP/1.1 200 OK\r\n" + "Content-Length: " + bodyMessage.Length + "\r\n\r\n";
            var headerBytes = Encoding.UTF8.GetBytes(messageHeaders);
            var combinedMessage = new byte[headerBytes.Length + bodyMessage.Length];
            CombineArrays(headerBytes, combinedMessage, bodyMessage);
            return combinedMessage;
        }

        private byte[] GetRangeResponse(Request request)
        {
            var bodyMessage = _fileContents.GetFileContents(FormattedFilePath(_fileContents, request.Uri));
            var requestRanges = GetRanges(request);
            var startingByte = Int32.Parse(requestRanges[0]);
            var endingByte = Int32.Parse(requestRanges[1]);
            var portionBodyMessage = GetPortionOfBody(bodyMessage, startingByte, endingByte);
            var messageHeaders = "HTTP/1.1 206 Partial Content\r\n" + "Content-Length: " + portionBodyMessage.Length
                                 + "\r\nContent-Range: bytes " + startingByte + "-" + endingByte + "\r\n\r\n";
            var headerBytes = Encoding.UTF8.GetBytes(messageHeaders);
            var combinedMessage = new byte[headerBytes.Length + portionBodyMessage.Length];
            CombineArrays(headerBytes, combinedMessage, portionBodyMessage);
            return combinedMessage;
        }

        private static bool HasRangeHeader(Request request)
        {
            return Array.Exists(request.Headers, element => element.StartsWith("Range: bytes="));
        }

        private static string[] GetRanges(Request request)
        {
            var item = request.Headers.First(element => element.StartsWith("Range: bytes="));
            var index = Array.IndexOf(request.Headers, item);
            return request.Headers[index].Substring(13).Split('-');
        }

        private byte[] GetPortionOfBody(byte[] bodyMessage, int startByte, int endByte)
        {
            byte[] portion = new byte[endByte-startByte+1];
            for (int index = 0; index < (endByte - startByte)+1; index++)
            {
                portion[index] = bodyMessage[index + startByte];
            }
            return portion;
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
