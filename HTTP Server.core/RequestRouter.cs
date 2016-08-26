using System.Collections.Generic;
namespace HTTPServer.core
{
    public class RequestRouter
    {
        private IHttpHandler _actions;

        public RequestRouter(IHttpHandler function)
        {
            _actions = function;
        }

        public byte[] HandleData(Request request, IPathContents pathContents)
        {
            return _actions.Execute(request);
        }
    }
}
