namespace HTTPServer.core
{
    public static class RequestHandler
    {
        public static void HandleData(ISocket clientConnection, IPathContents _pathContents)
        {
            byte[] message = RequestReader.Read(clientConnection);
            SendData(Reply.HandleReply(message, _pathContents), clientConnection);
        }

        private static int ReceiveData(byte[] buffer, ISocket clientConnection)
        {
            return clientConnection.Receive(buffer);
        }

        private static void SendData(byte[] buffer, ISocket clientConnection)
        {
            clientConnection.Send(buffer);
        }
    }
}
