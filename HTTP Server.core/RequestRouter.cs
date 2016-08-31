using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPServer.core
{
    public class RequestRouter
    {
        private List<Tuple<ICriteria, IHttpHandler>> _commandsAndCriteria;

        public RequestRouter()
        {
            _commandsAndCriteria = new List<Tuple<ICriteria, IHttpHandler>>();
        }
        public Reply HandleData(Request request, IPathContents pathContents)
        {
            if(IsWrongVersion(request))
            {
                return ReplyWrongVersion();
            }
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

        private bool IsWrongVersion(Request request)
        {  
        return IsValidMethod(request) && request.HttpVersion.Substring(0, 5).Equals("HTTP/") && !request.HttpVersion.Equals("HTTP/1.1");
        }

        private static bool IsValidMethod(Request request)
        {
            return request.Method.Equals("GET") || request.Method.Equals("POST") || request.Method.Equals("PUT");
        }

        private Reply ReplyWrongVersion()
        {
            var reply = new Reply();
            reply.StartingLine = Encoding.UTF8.GetBytes("HTTP/1.1 505 HTTP Version Not Supported\r\n");
            return reply;
        }
    }
}
