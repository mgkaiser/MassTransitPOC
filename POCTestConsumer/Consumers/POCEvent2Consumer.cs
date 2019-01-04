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
            var random = new Random(DateTime.Now.Millisecond);
            var resultId = random.Next(1, 1000);

            Console.WriteLine($"{context.Message.eventId} {resultId}");
            await context.RespondAsync<IPOCEvent2Response>(new { Response = context.Message.fail ? "Fail" : "Success", ResultId = resultId});           
        }
    }
}
