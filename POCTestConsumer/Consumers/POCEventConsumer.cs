using MassTransit;
using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POCTestConsumer.Consumers
{
    public class POCEventConsumer : IConsumer<IPOCEvent>
    {
        public async Task Consume(ConsumeContext<IPOCEvent> context)
        {
            await Task.Delay(10);
            Console.WriteLine($"{context.Message.LastName}, {context.Message.FirstName}");
            if (context.Message.Fail) throw new Exception();
        }
    }
}
