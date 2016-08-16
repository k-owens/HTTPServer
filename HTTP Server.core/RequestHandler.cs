namespace HTTPServer.core
{
    public static class RequestHandler
    {
        public static void HandleData(ISocket clientConnection, IDirectoryContents directoryContents, IFileContents fileContents)
        {
            byte[] message = RequestReader.Read(clientConnection);
            SendData(Reply.HandleReply(message, directoryContents, fileContents), clientConnection);
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
