using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server.core
{
    interface ISocket
    {
        void Bind(IPEndPoint ipEndPoint);
        void Listen(int backlog);
        ISocket Accept();
        void Close();
        void Send(byte[] buffer);
        void Receive(byte[] buffer);
    }
}
