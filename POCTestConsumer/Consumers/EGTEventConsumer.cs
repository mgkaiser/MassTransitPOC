using MassTransit;
using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POCTestConsumer.Consumers
{
    public class EGTEventConsumer : IConsumer<IEGTEvent>
    {
        public async Task Consume(ConsumeContext<IEGTEvent> context)
        {
            await Task.Delay(10);         
            Console.WriteLine($"{context.Message.SenderId}, {context.Message.EventType}");
            throw new Exception();
        }
    }
}
