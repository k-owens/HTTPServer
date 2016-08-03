using System.Net;
using System.Net.Sockets;

namespace HTTPServer.core
{
    public class NetworkSocket: ISocket
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

        public EndPoint LocalEndPoint()
        {
            return thisSocket.LocalEndPoint;
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

        public int Send(byte[] buffer)
        {
            return thisSocket.Send(buffer);
        }

        public int Receive(byte[] buffer)
        {
            return thisSocket.Receive(buffer);
        }
    }
}
