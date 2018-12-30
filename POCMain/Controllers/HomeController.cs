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
            var tasks = Enumerable.Range(0, model.Count).Select(i => Program.Bus.Publish<IPOCEvent>(new { LastName = model.LastName, FirstName = model.FirstName, Fail = model.Fail.ToString() }));
            await Task.WhenAll(tasks);
            return new JsonResult(new { status = "Success", result = 1 });
        }

        public async Task<JsonResult> DoAThing2([FromBody] POCViewModel model)
        {            
            var tasks = Enumerable.Range(0, model.Count).Select(i => Program.PocEvent2Client.Request<IPOCEvent2Request, IPOCEvent2Response>(new { eventId = 123, fail = model.Fail.ToString() }));
            var response = await Task.WhenAll(tasks);
            return new JsonResult(new { status = response.First().Response, result = response.First().ResultId });
        }

        public async Task<JsonResult> DoAThing3([FromBody] POCViewModel model)
        {            
            var tasks = Enumerable.Range(0, model.Count).Select(i => Program.Bus.Publish<IEGTEvent>(new { SenderId = "Serve.CAM.Events", EventType = "CAM.Customer.Created", Message = model.Fail.ToString() }));
            await Task.WhenAll(tasks);
            return new JsonResult(new { status = "Success", result = 1 });
        }
        public async Task<JsonResult> DoAThing4([FromBody] POCViewModel model)
        {
            var tasks = Enumerable.Range(0, model.Count).Select(i => Program.Bus.Publish<IEGTEvent>(new { SenderId = "Serve.TXP.Events", EventType = "TXP.Transaction.Complete", Message = model.Fail.ToString() }));
            await Task.WhenAll(tasks);
            return new JsonResult(new { status = "Success", result = 1 });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
