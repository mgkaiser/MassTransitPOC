using MassTransit;
using POCInterfaces;
using POCTestConsumer.Consumers;
using RabbitMQ.Client;
using System;
using GreenPipes;
using System.Threading;

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
            Thread.Sleep(Timeout.Infinite);

            _busControl.Stop(TimeSpan.FromSeconds(10));
        }

        private static IBusControl ConfigureBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://192.168.3.110"), h =>
                {
                    h.Username("mkaiser");
                    h.Password("mgk070294");
                });

                cfg.UseDelayedExchangeMessageScheduler();
                
                cfg.ReceiveEndpoint(host, "POCEventConsumer_queue", x =>
                {
                    x.Consumer<POCEventConsumer>(consumer => {
                        consumer.Message<IPOCEvent>(msg => msg.UseScheduledRedelivery(Retry.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))));
                    });
                    x.Consumer<POCEvent2Consumer>(consumer => {
                        consumer.Message<IPOCEvent2Request>(msg => msg.UseScheduledRedelivery(Retry.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))));
                    });
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
                    x.Consumer<EGTEventConsumer>(consumer => {
                        consumer.Message<IEGTEvent>(msg => msg.UseScheduledRedelivery(Retry.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))));
                    });
                    
                });

                cfg.ReceiveEndpoint(host, "POCEventConsumer_TXP_queue", x =>
                {
                    x.BindMessageExchanges = false;                    
                    x.Bind("POCInterfaces:IEGTEvent", config =>
                    {
                        config.ExchangeType = ExchangeType.Direct;
                        config.RoutingKey = "Serve.TXP.Events:TXP.Transaction.Complete";
                    });                    
                    x.Consumer<EGTEventConsumer>(consumer => {
                        consumer.Message<IEGTEvent>(msg => msg.UseScheduledRedelivery(Retry.Incremental(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))));
                    });
                });

            });
        }
    }
}
