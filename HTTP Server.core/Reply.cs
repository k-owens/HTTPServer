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
                string[] requestLine = requestMessage.Split(' ', ' ');
                method = requestLine[0];
                uri = requestLine[1];
                version = requestLine[2];
            }
            catch
            {
                return Encoding.UTF8.GetBytes(CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents));
            }
            if (version.Equals("HTTP/1.1\r\n") && IsRequestMethod(method))
            {
                if (uri.Equals("/"))
                    return Encoding.UTF8.GetBytes(CreateMessage("HTTP/1.1 200 OK\r\n", directoryContents));
                return Encoding.UTF8.GetBytes(CreateMessage("HTTP/1.1 404 Not Found\r\n", directoryContents));
            }
            return Encoding.UTF8.GetBytes(CreateMessage("HTTP/1.1 400 Bad Request\r\n", directoryContents));
        }
        
        private static bool IsRequestMethod(string str)
        {
            return str.Equals("GET");
        }

        public static string CreateMessage(string httpStartLine, IDirectoryContents directoryContents)
        {
            if (directoryContents.DirectoryPath.Equals(""))
                return httpStartLine;
            string[] files = directoryContents.GetFiles();
            string[] directories = directoryContents.GetDirectories();
            var wholeMessage = httpStartLine;
            var bodyMessage = "<html>\r\n" +
                             "<body>\r\n";
            foreach (var folder in directories)
            {
                bodyMessage += "<p>\r\n" + folder + "\r\n" + "</p>\r\n";
            }
            foreach (var file in files)
            {
                bodyMessage += "<p>\r\n" + file + "\r\n" + "</p>\r\n";
            }
            bodyMessage += "</body>\r\n" +
                       "</html>";

            wholeMessage += "Content-Length: " + bodyMessage.Length + "\r\n";
            wholeMessage += "\r\n";
            wholeMessage += bodyMessage;
            return wholeMessage;
        }
    }
}
