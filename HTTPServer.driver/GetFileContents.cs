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

        public Reply Execute(Request request)
        {
            if (request.Uri.Equals("/logs"))
                return ObtainFileContents(request, "../logs.txt");
            if (IsValidFile(request.Uri, _fileContents))
                return ObtainFileContents(request, FormattedFilePath(_fileContents, request.Uri));
            var reply = new Reply();
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n");
            return reply;
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

        private Reply ObtainFileContents(Request request, string file)
        {
            if (HasRangeHeader(request))
                return GetRangeResponse(request);
            return GetNormalResponse(request, file);
        }

        private Reply GetNormalResponse(Request request, string file)
        {
            var reply = new Reply();
            reply.Body = _fileContents.GetFileContents(file);
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n");
            var messageHeaders = "Content-Length: " + reply.Body.Length + "\r\n";
            reply.Headers = Encoding.UTF8.GetBytes(messageHeaders);
            return reply;
        }

        private Reply GetRangeResponse(Request request)
        {
            Reply reply = new Reply();
            var bodyMessage = _fileContents.GetFileContents(FormattedFilePath(_fileContents, request.Uri));
            var requestRanges = GetRanges(request);
            var startingByte = Int32.Parse(requestRanges[0]);
            var endingByte = Int32.Parse(requestRanges[1]);
            reply.Body = GetPortionOfBody(bodyMessage, startingByte, endingByte);
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 206 Partial Content\r\n");
            reply.Headers = Encoding.UTF8.GetBytes("Content-Length: " + reply.Body.Length
                                 + "\r\nContent-Range: bytes " + startingByte + "-" + endingByte + "\r\n");
            return reply;
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
