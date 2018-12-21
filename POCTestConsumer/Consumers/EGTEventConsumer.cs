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
        private const string TxpTransactionComplete = "TXP.Transaction.Complete";

        public async Task Consume(ConsumeContext<IEGTEvent> context)
        {
            await Task.Delay(1);         
            Console.WriteLine($"{context.Message.SenderId}, {context.Message.EventType}");
            if (bool.Parse(context.Message.Message)) throw new Exception();            
        }
    }
}
