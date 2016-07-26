using System;
using System.IO;
using System.Text;
using HTTP_Server.core;

namespace HTTPServer.test
{
    public class MockConnection : IConnection
    {
        public MemoryStream MStream = new MemoryStream();

        public int ReceiveResponse(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int SendReply(byte[] buffer)
        {
            var SWriter = new StreamWriter(MStream);
            SWriter.Write(Encoding.UTF8.GetString(buffer));
            SWriter.Flush();
            return (int)MStream.Length;
        }
    }
}
