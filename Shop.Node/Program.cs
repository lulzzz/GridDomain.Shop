﻿using System;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using Shop.Composition;
using System.ServiceProcess;
using System.Threading.Tasks;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using Serilog;
using Serilog.Events;

namespace Shop.Node
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Log.Logger = new ShopLogConfiguration().CreateLogger();
            ServiceRunner<ShopNode>.Run(config =>
                                              {
                                                  var name = config.GetDefaultName();
                                                  config.SetName("Shop node");
                                                  config.Service(serviceConfig =>
                                                                 {
                                                                     serviceConfig.ServiceFactory((extraArguments, c) =>
                                                                                                  {
                                                                                                      return new ShopNode(CreateGridNode());
                                                                                                  });
                                                                     serviceConfig.OnStart((service, extraArguments) =>
                                                                                           {
                                                                                               Console.WriteLine("Service {0} started", name);
                                                                                               service.Start();
                                                                                           });

                                                                     serviceConfig.OnStop(service =>
                                                                                          {
                                                                                              Console.WriteLine("Service {0} stopped", name);
                                                                                              service.Stop();
                                                                                          });

                                                                     serviceConfig.OnError(e =>
                                                                                           {
                                                                                               Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                                                                                           });
                                                                 });
                                              });

            Console.WriteLine("Press any key and enter to exit");
            Console.ReadLine();
        }

        
        private static GridDomainNode CreateGridNode()
        {
            var config = new AkkaConfiguration(new ShopNodeNetworkConfig(), new ShopNodeDbConfig());
            //Allow parameterised configuration here.
            var settings = new NodeSettings(() => config.CreateSystem()) {Log = Log.Logger};
            settings.Add(new ShopDomainConfiguration());
            Console.WriteLine($"Created shop node at {config.Network.Host} on port {config.Network.PortNumber}");
            return new GridDomainNode(settings);
        }
    }

    class ShopLogConfiguration : DefaultLoggerConfiguration
    {
        public ShopLogConfiguration()
        {
            WriteTo.Console();
        }
    }
}
