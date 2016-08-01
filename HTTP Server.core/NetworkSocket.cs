using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server.core
{
    class NetworkSocket: ISocket
    {
        private Socket thisSocket;

        public NetworkSocket()
        {
            thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public NetworkSocket(Socket socket)
        {
            thisSocket = socket;
        }

        public void Bind(IPEndPoint ipEndPoint)
        {
            thisSocket.Bind(ipEndPoint);
        }

        public void Listen(int backlog)
        {
            thisSocket.Listen(backlog);
        }

        public ISocket Accept()
        {
            Socket connection = thisSocket.Accept();
            return new NetworkSocket(connection);
        }

        public void Close()
        {
            thisSocket.Close();
        }

        public void Send(byte[] buffer)
        {
            thisSocket.Send(buffer);
        }

        public void Receive(byte[] buffer)
        {
            thisSocket.Receive(buffer);
        }
    }
}
