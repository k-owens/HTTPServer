using System;
using System.Net;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class MockConnection: ISocket
    {
        public EndPoint LocalEndPoint()
        {
            return new IPEndPoint(0,0);
        }

        public void Bind(IPEndPoint ipEndPoint)
        {
            
        }

        public void Listen(int backlog)
        {
            
        }

        public ISocket Accept()
        {
            return new MockConnection();
        }

        public void Close()
        {

        }

        public int Send(byte[] buffer)
        {
            return DataHolder.WriteData(buffer);
        }

        public int Receive(byte[] buffer)
        {
            return DataHolder.ReadData(buffer);
        }

        public void Connect(IPEndPoint ipEndPoint)
        {
            
        }
    }
}
