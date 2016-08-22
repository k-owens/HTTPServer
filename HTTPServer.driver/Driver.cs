using System;
using HTTPServer.core;
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
            var requestHandler = AddFunctionality();
            var info = new ServerInfo(port, new ConcretePathContents(directoryPath), requestHandler);
            server.Start(info);
            server.HandleClients();
        }

        private static void HandleCommands(string[] args)
        {
            for (var i = 0; i < args.Length && args[i][0] == '-'; i++)
            {
                switch (args[i][1])
                {
                    case 'p':
                        port = Int32.Parse(args[i + 1]);
                        break;
                    case 'd':
                        directoryPath = args[i + 1];
                        break;
                }
            }
        }

        private static RequestRouter AddFunctionality()
        {
            var pathContents = new ConcretePathContents(directoryPath);
            var requestHandler = new RequestRouter();
            requestHandler.AddAction(new GetDirectoryContents(pathContents));
            requestHandler.AddAction(new GetFileContents(pathContents));
            return requestHandler;
        }
    }
}
