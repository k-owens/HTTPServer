using System;
using System.Net;
using HTTPServer.core;

namespace HTTPServer.test
{
    public class MockConnection: ISocket
    {
        //what if it is in a neutral area that both can use?
        //MemoryStream ms = new MemoryStream(); // holds all of the data; works if one MockConnection acts as both ISockets

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
