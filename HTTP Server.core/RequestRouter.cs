using System;
using System.Collections.Generic;

namespace HTTPServer.core
{
    public class RequestRouter
    {
        private List<Tuple<ICriteria, IHttpHandler>> _commandsAndCriteria;

        public RequestRouter(List<Tuple<ICriteria, IHttpHandler>> commandsAndCriteria)
        {
            _commandsAndCriteria = commandsAndCriteria;
        }

        public Reply HandleData(Request request, IPathContents pathContents)
        {
            for(int i = 0; i < _commandsAndCriteria.Count-1; i++)
            {
                if (_commandsAndCriteria[i].Item1.ShouldRun(request))
                    return _commandsAndCriteria[i].Item2.Execute(request);
            }

            return _commandsAndCriteria[_commandsAndCriteria.Count-1].Item2.Execute(request);
        }
    }
}
