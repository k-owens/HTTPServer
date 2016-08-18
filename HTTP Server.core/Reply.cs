using System;
using System.Text;

namespace HTTPServer.core
{
    public class Reply
    {
        public byte[] CreateReply(string httpMethod, IPathContents pathContents, string uri)
        {
            switch (httpMethod)
            {
                case "200":
                    return BuildMessage("HTTP/1.1 200 OK\r\n", pathContents, uri);
                case "400":
                    return BuildMessage("HTTP/1.1 400 Bad Request\r\n", pathContents, uri);
                case "404":
                    return BuildMessage("HTTP/1.1 404 Not Found\r\n", pathContents, uri);
                case "505":
                    return BuildMessage("HTTP/1.1 505 HTTP Version Not Supported\r\n", pathContents, uri);
                default:
                    return BuildMessage("HTTP/1.1 400 Bad Request\r\n", pathContents, uri);
            }
        }

        private byte[] BuildMessage(string httpStartLine, IPathContents pathContents, string uri)
        {
            if (HasNoDirectoryPath(pathContents) || !Is200Message(httpStartLine))
                return Encoding.UTF8.GetBytes(httpStartLine);
            if (IsRoot(uri))
                return Encoding.UTF8.GetBytes(GetHtmlMessage(pathContents, httpStartLine));
            return GetContentsOfFile(pathContents, uri, httpStartLine);
        }

        private static bool IsRoot(string uri)
        {
            return uri.Equals("/");
        }

        private static bool Is200Message(string httpStartLine)
        {
            return httpStartLine.Substring(9,3).Equals("200");
        }

        private static bool HasNoDirectoryPath(IPathContents pathContents)
        {
            return pathContents.DirectoryPath.Equals("");
        }

        private byte[] GetContentsOfFile(IPathContents pathContents, string uri,
            string httpStartLine)
        {
            var bodyMessage = pathContents.GetFileContents(FormattedFilePath(pathContents, uri));
            var messageHeaders = httpStartLine + "Content-Length: " + bodyMessage.Length + "\r\n\r\n";
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

        private string GetHtmlMessage(IPathContents pathContents, string httpStartLine)
        {
            var bodyMessage = GetBodyOfMessage(pathContents.GetDirectories(), pathContents.GetFiles());
            var wholeMessage = httpStartLine + "Content-Length: " + bodyMessage.Length + "\r\n\r\n";
            wholeMessage += bodyMessage;
            return wholeMessage;
        }

        private string GetBodyOfMessage(string[] directories, string[] files)
        {
            var bodyMessage = "<html>" +
                              "<body>";
            foreach (var folder in directories)
            {
                bodyMessage += "<p>" + folder + "</p>";
            }
            foreach (var file in files)
            {
                bodyMessage += "<p>" + file + "</p>";
            }
            bodyMessage += "</body>" +
                           "</html>";
            return bodyMessage;
        }
    }
}
