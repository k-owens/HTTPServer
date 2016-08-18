using System.Text;

namespace HTTPServer.core
{
    public class RequestHandler
    {
        private string _uri;
        private string _method;
        private string _version;
        private Reply _reply;

        public byte[] HandleData(byte[] message, IPathContents pathContents)
        {
            _reply = new Reply();
            var requestMessage = Encoding.UTF8.GetString(message).Substring(0, message.Length);
            try
            {
                DivideStartLine(requestMessage);
            }
            catch
            {
                return _reply.CreateReply("400", pathContents, _uri);
            }

            return MethodResponse(pathContents);
        }

        private void DivideStartLine(string requestMessage)
        {
            SetStartLineProperties(SplitLine(requestMessage));
        }

        private static string[] SplitLine(string requestMessage)
        {
            var endIndexOfFirstLine = requestMessage.IndexOf('\n');
            var firstLine = requestMessage.Substring(0, endIndexOfFirstLine + 1);
            string[] requestLine = firstLine.Split(' ', ' ');
            return requestLine;
        }

        private void SetStartLineProperties(string[] requestLine)
        {
            _method = requestLine[0];
            _uri = requestLine[1];
            _version = requestLine[2];
        }

        private byte[] MethodResponse(IPathContents pathContents)
        {
            switch (_method)
            {
                case "GET":
                    return GetMethodResponse(pathContents);
                default:
                    return _reply.CreateReply("400", pathContents, _uri);
            }
        }

        private byte[] GetMethodResponse(IPathContents pathContents)
        {
            if (IsValidVersionSection(_version) && IsRequestMethod(_method))
            {
                if (IsRoot(_uri) || IsValidFile(_uri, pathContents))
                    return _reply.CreateReply("200", pathContents, _uri);
                return _reply.CreateReply("404", pathContents, _uri);
            }
            if (!IsSupportedVersion(_version))
                return _reply.CreateReply("505", pathContents, _uri);
            return _reply.CreateReply("400", pathContents, _uri);
        }

        private bool IsRequestMethod(string requestMethod)
        {
            return requestMethod.Equals("GET") || requestMethod.Equals("POST");
        }
        private bool IsValidVersionSection(string version)
        {
            return version.Equals("HTTP/1.1\r\n");
        }

        private bool IsSupportedVersion(string version)
        {
            return version.Substring(version.Length - 5, 3).Equals("1.1");
        }

        private bool IsRoot(string uri)
        {
            return uri.Equals("/");
        }

        private bool IsValidFile(string uri, IPathContents pathContents)
        {
            try
            {
                string[] files = pathContents.GetFiles();
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
    }
}
