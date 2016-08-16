using System.Text;

namespace HTTPServer.core
{
    public static class Reply
    {
        public static byte[] HandleReply(byte[] request, IDirectoryContents directoryContents)
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
                return CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents);
            }
            if (version.Equals("HTTP/1.1\r\n") && IsRequestMethod(method))
            {
                if (uri.Equals("/"))
                    return CreateMessage("HTTP/1.1 200 OK\r\n", directoryContents);
                return CreateMessage("HTTP/1.1 404 Not Found\r\n", directoryContents);
            }
            return CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents);
        }
        
        private static bool IsRequestMethod(string str)
        {
            return str.Equals("GET");
        }

        public static byte[] CreateMessage(string httpStartLine, IDirectoryContents directoryContents)
        {
            if (directoryContents.DirectoryPath.Equals(""))
                return Encoding.UTF8.GetBytes(httpStartLine);
            string[] files = directoryContents.GetFiles();
            string[] directories = directoryContents.GetDirectories();
            var wholeMessage = httpStartLine;
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

            wholeMessage += "Content-Length: " + bodyMessage.Length + "\r\n";
            wholeMessage += "\r\n";
            wholeMessage += bodyMessage;
            return Encoding.UTF8.GetBytes(wholeMessage);
        }
    }
}
