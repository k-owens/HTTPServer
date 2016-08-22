using System.Collections.Generic;
using System.Text;


namespace HTTPServer.core
{
    public class RequestRouter
    {
        private Request _request;
        private List<IHttpHandler> _actions = new List<IHttpHandler>();

        public void AddAction(IHttpHandler function)
        {
            _actions.Add(function);
        }

        public byte[] HandleData(Request request, IPathContents pathContents)
        {
            _request = request;
            return DetermineAction(_request, pathContents);
        }

        private byte[] DetermineAction(Request request, IPathContents pathContents)
        {
            for (var listIndex = 0; listIndex < _actions.Count - 1; listIndex++)
            {
                if (_actions[listIndex].ShouldRun(request, pathContents))
                    return _actions[listIndex].Execute(request);
            }
            return _actions[_actions.Count - 1].Execute(request);
        }
    }
}
