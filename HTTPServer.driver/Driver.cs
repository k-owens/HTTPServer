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

        public static void Main(string[] args)
        {
            server.Start(HandlePort(args));
            server.HandleClients();

        }

        private static int HandlePort(string[] args)
        {
            int port;
            if (args.Length == 2)
            {
                if (args[0].Equals("-p"))
                    port = Int32.Parse(args[1]);
                else
                    throw new Exception();
            }
            else
            {
                port = 8080;
            }
            return port;
        }
    }
}
