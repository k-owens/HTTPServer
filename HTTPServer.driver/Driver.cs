using System;
using System.Text;
using HTTPServer.core;
using System.Net;
using System.Net.Sockets;

namespace HTTPServer.driver
{
    public class Driver
    {
        private static Server server = new Server();
        private static byte[] bytesToSend;
        private static byte[] bytesReturned;
        private static IPAddress ipAddress;
        private static IPEndPoint ipEndPoint;
        private static Socket socket;

        public static void Main(string[] args)
        {
            SetUpDriverClient();
            ConnectClientToServer(server, socket, ipEndPoint);
            bytesToSend = GetDataToSend(bytesToSend);
            var message = CommunicateWithServer(socket, bytesToSend, server, bytesReturned);
            Console.WriteLine("The message that was received by the server is: " + message);
            CloseConnectionToServer(server, socket);
            KeepWindowOpen();
        }

        private static void SetUpDriverClient()
        {
            server = new Server();
            bytesToSend = new byte[1024];
            bytesReturned = new byte[1024];
            ipAddress = IPAddress.Parse("127.0.0.1");
            ipEndPoint = new IPEndPoint(ipAddress, 8080);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static void KeepWindowOpen()
        {
            Console.ReadKey();
        }

        private static void CloseConnectionToServer(Server server, Socket socket)
        {
            server.Stop();
            socket.Close();
            Console.WriteLine("Client has disconnected.");
        }

        private static string CommunicateWithServer(Socket socket, byte[] bytesToSend, Server server, byte[] bytesReturned)
        {
            socket.Send(bytesToSend);
            Console.WriteLine("Client has sent data to the server.");
            server.HandleData();
            socket.Receive(bytesReturned);
            Console.WriteLine("Client has received data back.");
            var message = Encoding.UTF8.GetString(bytesReturned, 0, bytesToSend.Length);
            return message;
        }

        private static byte[] GetDataToSend(byte[] bytesToSend)
        {
            Console.WriteLine("Please enter a message to be sent to the server:");
            var userMessage = Console.ReadLine();
            bytesToSend = Encoding.UTF8.GetBytes(userMessage);
            return bytesToSend;
        }

        private static void ConnectClientToServer(Server server, Socket socket, IPEndPoint ipEndPoint)
        {
            server.Start();
            socket.Connect(ipEndPoint);
            Console.WriteLine("Client has connected to the end point.");
            server.ConnectToClient();
        }
    }
}
