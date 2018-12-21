using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Mvc;
using POCInterfaces;
using POCMain.Models;

namespace POCMain.Controllers
{
    public class HomeController : Controller
    {        
   
        public IActionResult Index()
        {
            return View("POC", new POCViewModel() { LastName = "Poop" });
        }

        public IActionResult POC()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<JsonResult> DoAThing([FromBody] POCViewModel model)
        {
            await Program.Bus.Publish<IPOCEvent>(new { model.LastName, model.FirstName });
            
            return new JsonResult(new { status = "Success" });
        }

        public async Task<JsonResult> DoAThing2()
        {
            var response = await Program.PocEvent2Client.Request<IPOCEvent2Request, IPOCEvent2Response>(new { eventId = 123 });
            return new JsonResult(new { status = response.Response });
        }

        public async Task<JsonResult> DoAThing3()
        {
            await Program.Bus.Publish<IEGTEvent>(new { SenderId = "Serve.CAM.Events", EventType = "CAM.Customer.Created", Message="TooCool!" });

            return new JsonResult(new { status = "Success" });
        }
        public async Task<JsonResult> DoAThing4()
        {
            await Program.Bus.Publish<IEGTEvent>(new { SenderId = "Serve.TXP.Events", EventType = "TXP.Transaction.Complete", Message = "TooCool!" });

            return new JsonResult(new { status = "Success" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
