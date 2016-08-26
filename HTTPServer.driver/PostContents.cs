using System.Text;
using HTTPServer.core;


namespace HTTPServer.app
{
    public class PostContents : IHttpHandler
    {
        private IPathContents _pathContents;

        public PostContents(IPathContents pathContents)
        {
            _pathContents = pathContents;
        }

        public byte[] Execute(Request request)
        {
            return Post(request);
        }

        public bool ShouldRun(Request request, IPathContents pathContents)
        {
            return request.Method.Equals("POST") && request.HttpVersion.Equals("HTTP/1.1\r\n") && !request.Uri.Equals("/");
        }

        private byte[] Post(Request request)
        {
            if (!IsValidFile(request.Uri, _pathContents))
            {
                try
                {
                    _pathContents.PostContents(request);
                    return Encoding.UTF8.GetBytes("HTTP/1.1 201 Created\r\n");
                }
                catch
                {
                    return Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\n");
                }
            }
            else
                return Encoding.UTF8.GetBytes("HTTP/1.1 409 Conflict\r\n");
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
    }
}
