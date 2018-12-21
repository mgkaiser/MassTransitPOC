using MassTransit;
using POCInterfaces;
using POCTestConsumer.Consumers;
using RabbitMQ.Client;
using System;
using GreenPipes;

namespace POCTestConsumer
{
    class Program
    {
        private static IBusControl _busControl;

        public static IBus Bus
        {
            get { return _busControl; }
        }

        static void Main(string[] args)
        {
            _busControl = ConfigureBus();
            _busControl.Start();    

            Console.WriteLine("POCTestConsumer starting...");
            Console.ReadLine();

            _busControl.Stop(TimeSpan.FromSeconds(10));
        }

        private static IBusControl ConfigureBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://192.168.0.12"), h =>
                {
                    h.Username("mkaiser");
                    h.Password("mgk070294");
                });

                cfg.UseDelayedExchangeMessageScheduler();
                
                cfg.ReceiveEndpoint(host, "POCEventConsumer_queue", x =>
                {
                    x.Consumer<POCEventConsumer>();
                    x.Consumer<POCEvent2Consumer>();
                });

                cfg.ReceiveEndpoint(host, "POCEventConsumer_CAM_queue", x =>
                {
                    x.BindMessageExchanges = false;                
                    x.Bind("POCInterfaces:IEGTEvent", config =>
                    {
                        config.ExchangeType = ExchangeType.Direct;
                        config.RoutingKey = "Serve.CAM.Events:CAM.Customer.Created";
                    });
                    x.Bind("POCInterfaces:IEGTEvent", config =>
                    {
                        config.ExchangeType = ExchangeType.Direct;
                        config.RoutingKey = "Serve.CAM.Events:CAM.Card.Created";
                    });
                    //x.UseRetry(retryConfig => retryConfig.Exponential(10, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(100), TimeSpan.FromMinutes(10)));
                    x.UseRetry(retryConfig => retryConfig.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
                    x.Consumer<EGTEventConsumer>();
                    
                });

                cfg.ReceiveEndpoint(host, "POCEventConsumer_TXP_queue", x =>
                {
                    x.BindMessageExchanges = false;                    
                    x.Bind("POCInterfaces:IEGTEvent", config =>
                    {
                        config.ExchangeType = ExchangeType.Direct;
                        config.RoutingKey = "Serve.TXP.Events:TXP.Transaction.Complete";
                    });
                    x.UseRetry(retryConfig => retryConfig.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
                    x.Consumer<EGTEventConsumer>();                   
                });

            });
        }
    }
}
