using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer.core
{
    public interface ISocket
    {
        EndPoint LocalEndPoint();
        void Bind(IPEndPoint ipEndPoint);
        void Listen(int backlog);
        ISocket Accept();
        void Close();
        int Send(byte[] buffer);
        int Receive(byte[] buffer);
        void Connect(IPEndPoint ipEndPoint);
    }
}
