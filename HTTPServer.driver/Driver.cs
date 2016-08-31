using System;
using HTTPServer.core;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HTTPServer.app
{
    public class Driver
    {
        private static Server server = new Server();
        private static int port = 0;
        private static string directoryPath = "";

        public static void Main(string[] args)
        {
            
            HandleCommands(args);
            var pathContents = new ConcretePathContents(directoryPath);
            var requestHandler = AddFunctionality(pathContents);
            var info = new ServerInfo(port, pathContents,requestHandler);
            server.Start(info);
            server.HandleClients();
        }

        private static RequestRouter AddFunctionality(IPathContents pathContents)
        {
            var requestHandler = new RequestRouter();
            requestHandler.AddAction(new BadRequestCriteria(),new BadRequestErrorMessage(pathContents));
            requestHandler.AddAction(new ContentsCriteria(), new GetContents(pathContents));
            requestHandler.AddAction(new PostCriteria(), new PostContents(pathContents));
            requestHandler.AddAction(new PutCriteria(), new PutContents(pathContents));
            return requestHandler;
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
