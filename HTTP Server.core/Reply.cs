using System;
using System.Linq;
using System.Text;

namespace HTTPServer.core
{
    public static class Reply
    {
        public static byte[] HandleReply(byte[] request, IDirectoryContents directoryContents, IFileContents fileContents)
        {
            var requestMessage = Encoding.UTF8.GetString(request).Substring(0, request.Length);
            var uri = "";
            var method = "";
            var version = "";
            try
            {
                var endIndexOfFirstLine = requestMessage.IndexOf('\n');
                var firstLine = requestMessage.Substring(0, endIndexOfFirstLine + 1);
                string[] requestLine = firstLine.Split(' ', ' ');
                method = requestLine[0];
                uri = requestLine[1];
                version = requestLine[2];
            }
            catch
            {
                return CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents, uri, fileContents);
            }
            if (version.Equals("HTTP/1.1\r\n") && IsRequestMethod(method))
            {
                if (uri.Equals("/") || IsValidFile(uri,directoryContents))
                    return CreateMessage("HTTP/1.1 200 OK\r\n", directoryContents, uri, fileContents);
                return CreateMessage("HTTP/1.1 404 Not Found\r\n", directoryContents, uri, fileContents);
            }
            return CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents, uri, fileContents);
        }

        private static bool IsValidFile(string uri, IDirectoryContents directoryContents)
        {
            try
            {
                string[] files = directoryContents.GetFiles();
                foreach (string file in files)
                {
                    var expectedFilePath = GetExpectedFilePath(uri, directoryContents);
                    if ((expectedFilePath).Equals(file))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static string GetExpectedFilePath(string uri, IDirectoryContents directoryContents)
        {
            var expectedFilePath = directoryContents.DirectoryPath;
            expectedFilePath += "\\";
            expectedFilePath += uri.Substring(1);
            return expectedFilePath;
        }

        private static bool IsRequestMethod(string str)
        {
            return str.Equals("GET");
        }

        public static byte[] CreateMessage(string httpStartLine, IDirectoryContents directoryContents, string uri, IFileContents fileContents)
        {
            if (directoryContents.DirectoryPath.Equals("") || !httpStartLine.Substring(9,3).Equals("200"))
                return Encoding.UTF8.GetBytes(httpStartLine);
            var wholeMessage = httpStartLine;
            byte[] replyMessage;
            if (uri.Equals("/"))
            {
                replyMessage = Encoding.UTF8.GetBytes(GetHtmlMessage(directoryContents, wholeMessage));
            }
            else
            {
                replyMessage = GetContentsOfFile(directoryContents, uri, fileContents, wholeMessage); ;
            }
            return replyMessage;
        }

        private static byte[] GetContentsOfFile(IDirectoryContents directoryContents, string uri, IFileContents fileContents,
            string wholeMessage)
        {
            string actualFilePath = directoryContents.DirectoryPath;
            actualFilePath += "\\";
            actualFilePath += uri.Substring(1);
            var bodyMessage = fileContents.GetFileContents(actualFilePath);
            wholeMessage += "Content-Length: " + bodyMessage.Length + "\r\n";
            wholeMessage += "\r\n";

            byte[] messageBytes = Encoding.UTF8.GetBytes(wholeMessage);
            byte[] combinedMessage = new byte[messageBytes.Length + bodyMessage.Length];
            Buffer.BlockCopy(messageBytes, 0, combinedMessage, 0, messageBytes.Length);
            Buffer.BlockCopy(bodyMessage, 0, combinedMessage, messageBytes.Length, bodyMessage.Length);
            return combinedMessage;
        }

        private static string GetHtmlMessage(IDirectoryContents directoryContents, string wholeMessage)
        {
            string[] files = directoryContents.GetFiles();
            string[] directories = directoryContents.GetDirectories();
            var bodyMessage = GetBodyOfMessage(directories, files);
            wholeMessage += "Content-Length: " + bodyMessage.Length + "\r\n";
            wholeMessage += "\r\n";
            wholeMessage += bodyMessage;
            return wholeMessage;
        }

        private static string GetBodyOfMessage(string[] directories, string[] files)
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
