﻿using System;
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
            var pathContents = new ConcretePathContents("C:\\gitwork\\HTTP Server");
            var requestHandler = AddFunctionality(pathContents);
            var info = new ServerInfo(8080, pathContents,requestHandler);
            server.Start(info);
            server.HandleClients();
        }

        private static IHttpHandler AddFunctionality(IPathContents pathContents)
        {
            var requestRouter = new RequestRouter(pathContents);
            requestRouter.AddAction(new ContentsCriteria(), new GetContents(pathContents));
            requestRouter.AddAction(new PostCriteria(), new PostContents(pathContents));
            requestRouter.AddAction(new PutCriteria(), new PutContents(pathContents));
            requestRouter.AddAction(new DeleteCriteria(), new DeleteContents(pathContents));
            IHttpHandler versionFilter = new VersionNotSupportedFilter(requestRouter);
            IHttpHandler malformedFilter = new BadRequestFilter(pathContents, versionFilter);
            return malformedFilter;
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
