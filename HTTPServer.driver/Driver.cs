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
            List<Tuple<ICriteria, IHttpHandler>> commandDetails = AddFunctionality(pathContents);
            var requestHandler = new RequestRouter(commandDetails);

            var info = new ServerInfo(port, pathContents,requestHandler);
            server.Start(info);
            server.HandleClients();
        }

        private static List<Tuple<ICriteria, IHttpHandler>> AddFunctionality(IPathContents pathContents)
        {
            List<Tuple<ICriteria, IHttpHandler>> commandDetails = new List<Tuple<ICriteria, IHttpHandler>>();

            commandDetails.Add(Tuple.Create((ICriteria)new BadRequestCriteria(),(IHttpHandler)new BadRequestErrorMessage(pathContents)));
            commandDetails.Add(Tuple.Create((ICriteria)new VersionNotSupportedCriteria(), (IHttpHandler)new VersionNotSupported()));
            commandDetails.Add(Tuple.Create((ICriteria)new ContentsCriteria(), (IHttpHandler)new GetContents(pathContents)));
            commandDetails.Add(Tuple.Create((ICriteria)new PostCriteria(), (IHttpHandler)new PostContents(pathContents)));
            return commandDetails;
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
