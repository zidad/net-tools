using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using SampleMessages.ConcertManager;

namespace SampleConsoleApplication.ConcertManager
{
    //Bus.Publish (new ReserveTicket { Amount=4, Concert = “Kraftwerk” })
    public class ConcertManagerConsoleWriter : IConsume<ReserveTicket>
    {
	    public void Consume(ReserveTicket message)
	    {
	        Console.WriteLine(message);
	    }
    }
   
}
