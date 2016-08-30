using HTTPServer.core;

namespace HTTPServer.app
{
    public class BadRequestCriteria : ICriteria
    {
        public bool ShouldRun(Request request)
        {
            return !IsValidMethod(request) || !request.HttpVersion.Substring(0, 5).Equals("HTTP/");
        }

        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST");
        }
    }
}
