using MassTransit;
using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POCTestConsumer.Consumers
{
    public class POCEvent2Consumer : IConsumer<IPOCEvent2Request>
    {
        public async Task Consume(ConsumeContext<IPOCEvent2Request> context)
        {
            Console.WriteLine($"{context.Message.eventId}");
            await context.RespondAsync<IPOCEvent2Response>(new { Response = context.Message.fail ? "Fail" : "Success" });           
        }
    }
}
