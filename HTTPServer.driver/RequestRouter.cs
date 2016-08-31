using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPServer.core
{
    public class RequestRouter : IHttpHandler
    {
        private List<Tuple<ICriteria, IHttpHandler>> _commandsAndCriteria;
        IPathContents _pathContents;

        public RequestRouter(IPathContents pathContents)
        {
            _commandsAndCriteria = new List<Tuple<ICriteria, IHttpHandler>>();
            _pathContents = pathContents;
        }
        public Reply Execute(Request request)
        {
            for(int i = 0; i < _commandsAndCriteria.Count-1; i++)
            {
                if (_commandsAndCriteria[i].Item1.ShouldRun(request))
                    return _commandsAndCriteria[i].Item2.Execute(request);
            }

            return _commandsAndCriteria[_commandsAndCriteria.Count-1].Item2.Execute(request);
        }

        public void AddAction(ICriteria criteria, IHttpHandler httpHandler)
        {
            _commandsAndCriteria.Add(Tuple.Create(criteria, httpHandler));
        }
    }
}
