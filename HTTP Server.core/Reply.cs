using System;
using System.Linq;
using System.Text;

namespace HTTPServer.core
{
    public static class Reply
    {
        public static byte[] HandleReply(byte[] request, IPathContents _pathContents)
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
                return CreateMessage("HTTP/1.1 400 Bad Request\r\n", _pathContents, uri);
            }
            if (IsValidVersionSection(version) && IsRequestMethod(method))
            {
                if (IsRoot(uri) || IsValidFile(uri,_pathContents))
                    return CreateMessage("HTTP/1.1 200 OK\r\n", _pathContents, uri);
                return CreateMessage("HTTP/1.1 404 Not Found\r\n", _pathContents, uri);
            }
            if (!IsSupportedVersion(version))
                return CreateMessage("HTTP/1.1 505 HTTP Version Not Supported\r\n", _pathContents, uri);
            return CreateMessage("HTTP/1.1 400 Bad Request\r\n", _pathContents, uri);
        }

        private static bool IsValidVersionSection(string version)
        {
            return version.Equals("HTTP/1.1\r\n");
        }

        private static bool IsSupportedVersion(string version)
        {
            return version.Substring(version.Length - 5, 3).Equals("1.1");
        }

        private static bool IsRoot(string uri)
        {
            return uri.Equals("/");
        }

        private static bool IsValidFile(string uri, IPathContents _pathContents)
        {
            try
            {
                string[] files = _pathContents.GetFiles();
                foreach (string file in files)
                {
                    var expectedFilePath = GetExpectedFilePath(uri, _pathContents);
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

        private static string GetExpectedFilePath(string uri, IPathContents _pathContents)
        {
            var expectedFilePath = _pathContents.DirectoryPath;
            expectedFilePath += "\\";
            expectedFilePath += uri.Substring(1);
            return expectedFilePath;
        }

        private static bool IsRequestMethod(string str)
        {
            return str.Equals("GET");
        }

        public static byte[] CreateMessage(string httpStartLine, IPathContents _pathContents, string uri)
        {
            if (_pathContents.DirectoryPath.Equals("") || !httpStartLine.Substring(9,3).Equals("200"))
                return Encoding.UTF8.GetBytes(httpStartLine);
            var wholeMessage = httpStartLine;
            byte[] replyMessage;
            if (uri.Equals("/"))
            {
                replyMessage = Encoding.UTF8.GetBytes(GetHtmlMessage(_pathContents, wholeMessage));
            }
            else
            {
                replyMessage = GetContentsOfFile(_pathContents, uri, wholeMessage); ;
            }
            return replyMessage;
        }

        private static byte[] GetContentsOfFile(IPathContents _pathContents, string uri,
            string wholeMessage)
        {
            string actualFilePath = _pathContents.DirectoryPath;
            actualFilePath += "\\";
            actualFilePath += uri.Substring(1);
            var bodyMessage = _pathContents.GetFileContents(actualFilePath);
            wholeMessage += "Content-Length: " + bodyMessage.Length + "\r\n";
            wholeMessage += "\r\n";

            byte[] messageBytes = Encoding.UTF8.GetBytes(wholeMessage);
            byte[] combinedMessage = new byte[messageBytes.Length + bodyMessage.Length];
            Buffer.BlockCopy(messageBytes, 0, combinedMessage, 0, messageBytes.Length);
            Buffer.BlockCopy(bodyMessage, 0, combinedMessage, messageBytes.Length, bodyMessage.Length);
            return combinedMessage;
        }

        private static string GetHtmlMessage(IPathContents _pathContents, string wholeMessage)
        {
            string[] files = _pathContents.GetFiles();
            string[] directories = _pathContents.GetDirectories();
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
