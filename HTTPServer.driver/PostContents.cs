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

        public Reply Execute(Request request)
        {
            return Post(request);
        }

        private Reply Post(Request request)
        {
            if (!IsValidFile(request.Uri, _pathContents))
            {
                var reply = new Reply();
                try
                {
                    _pathContents.PostContents(request);
                    reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 201 Created\r\n");
                }
                catch
                {
                    reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request\r\n");
                }
                return reply;
            }
            else
            {
                var reply = new Reply();
                reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 409 Conflict\r\n");
                return reply;
            }
        }

        private bool IsValidFile(string uri, IPathContents pathContents)
        {
            try
            {
                var files = pathContents.GetFiles("");
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
