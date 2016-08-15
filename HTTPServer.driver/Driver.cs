using System;
using HTTPServer.core;

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
            var info = new ServerInfo(port, new NetworkSocket(), new ConcreteDirectoryContents(directoryPath));
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
    }
}
