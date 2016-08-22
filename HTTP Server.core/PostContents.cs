using System.Text;

namespace HTTPServer.core
{
    public class PostContents : IFunctionality
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
            _pathContents.PostContents(request);
            return Encoding.UTF8.GetBytes("HTTP/1.1 201 Created\r\n");
        }
    }
}
