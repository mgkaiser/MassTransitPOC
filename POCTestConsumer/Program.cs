using MassTransit;
using POCInterfaces;
using POCTestConsumer.Consumers;
using RabbitMQ.Client;
using System;
using GreenPipes;
using System.Threading;
using System.Runtime.Loader;

namespace POCTestConsumer
{
    class Program
    {
        private static ManualResetEvent _Shutdown = new ManualResetEvent(false);
        private static ManualResetEventSlim _Complete = new ManualResetEventSlim();

        private static IBusControl _busControl;

        public static IBus Bus
        {
            get { return _busControl; }
        }

        static int Main(string[] args)
        {
            try
            {
                var ended = new ManualResetEventSlim();
                var starting = new ManualResetEventSlim();

                AssemblyLoadContext.Default.Unloading += Default_Unloading;

                Console.WriteLine("POCTestConsumer starting service bus ...");
                _busControl = ConfigureBus();
                _busControl.Start();    

                Console.WriteLine("POCTestConsumer starting...");
                
                // Wait for a singnal
                _Shutdown.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("POCTestConsumer shutting down service bus ...");
                _busControl.Stop(TimeSpan.FromSeconds(10));
            }

            Console.WriteLine("POCTestConsumer ending...");
            _Complete.Set();
 
            return 0;
            
        }

         private static void Default_Unloading(AssemblyLoadContext obj)
        {
            Console.WriteLine($"POCTestConsumer received SIGTERM...");
            _Shutdown.Set();
            _Complete.Wait();
        }

        private static IBusControl ConfigureBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://192.168.3.110"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
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
