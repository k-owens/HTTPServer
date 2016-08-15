namespace HTTPServer.core
{
    public class ClientConnector
    {
        public static ISocket ConnectToClient(ISocket socket)
        {
            var clientConnection = socket.Accept();
            return clientConnection;
        }
    }
}
