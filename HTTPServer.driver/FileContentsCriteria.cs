using HTTPServer.core;

namespace HTTPServer.app
{
    public class FileContentsCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return request.Method.Equals("GET") && !request.Uri.Equals("/");
        }
    }
}
