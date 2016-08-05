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
        private static int port = 0;
        private static string directoryPath = "";

        public static void Main(string[] args)
        {
            HandleCommands(args);
            server.Start(port, new NetworkSocket(), directoryPath);
            server.HandleClients();

        }

        private static void HandleCommands(string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].Equals("-p"))
                    port = Int32.Parse(args[1]);
                else if (args[0].Equals("-d"))
                {
                    directoryPath = args[1];
                }
                else
                    throw new Exception();
            }
            else if (args.Length == 4)
            {
                if (args[0].Equals("-p") && args[2].Equals("-d"))
                {
                    port = Int32.Parse(args[1]);
                    directoryPath = args[3];
                }
                else if (args[0].Equals("-d") && args[2].Equals("-p"))
                {
                    port = Int32.Parse(args[3]);
                    directoryPath = args[1];
                }
            }
        }
    }
}
