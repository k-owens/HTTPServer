using System.Collections.Generic;

namespace HTTPServer.core
{
    public static class RequestReader
    {
        public static byte[] Read(ISocket clientConnection)
        {
            List<byte> messageReceived = new List<byte>();
            while (true)
            {
                byte[] buffer = new byte[1024];
                var bytesReceived = ReceiveData(buffer, clientConnection);
                for (var bufferIndex = 0; bufferIndex < bytesReceived; bufferIndex++)
                {
                    messageReceived.Add(buffer[bufferIndex]);
                }
                if (bytesReceived < 1024)
                    break;
            }

            byte[] message = new byte[messageReceived.Count];
            for (var copyIndex = 0; copyIndex < messageReceived.Count; copyIndex++)
            {
                message[copyIndex] = messageReceived[copyIndex];
            }
            return message;
        }

        private static int ReceiveData(byte[] buffer, ISocket clientConnection)
        {
            return clientConnection.Receive(buffer);
        }
    }
}
