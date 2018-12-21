using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using POCInterfaces;
using RabbitMQ.Client;

namespace POCMain
{
    public class Program
    {
        private static IBusControl _busControl;

        public static IBus Bus
        {
            get { return _busControl; }
        }

        public static IRequestClient<IPOCEvent2Request, IPOCEvent2Response> PocEvent2Client;

        public static void Main(string[] args)
        {
            _busControl = ConfigureBus();
            _busControl.Start();

            PocEvent2Client = new MessageRequestClient<IPOCEvent2Request, IPOCEvent2Response>(Bus, new Uri("rabbitmq://192.168.0.12/POCEventConsumer_queue"), TimeSpan.FromSeconds(30));

            CreateWebHostBuilder(args).Build().Run();

            _busControl.Stop(TimeSpan.FromSeconds(10));
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static IBusControl ConfigureBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://192.168.0.12"), h =>
                {
                    h.Username("mkaiser");
                    h.Password("mgk070294");
                });

                cfg.Send<IEGTEvent>(x => {
                    x.UseRoutingKeyFormatter(context => {
                        var routingKey = $"{context.Message.SenderId}:{context.Message.EventType}";
                        System.Diagnostics.Debug.WriteLine($"RoutingKey={routingKey}");
                        return routingKey;
                    });
                });

                cfg.Publish<IEGTEvent>(x => x.ExchangeType = ExchangeType.Direct);
            });
        }
    }
}
