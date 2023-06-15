using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WeGotKicks.Controllers
{
    public class ZapawikiController : Controller
    {
        private readonly ILogger<ZapawikiController> _logger;

        public ZapawikiController(ILogger<ZapawikiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
          var random=new Random();
          var client = new HttpClient();
          var request = new HttpRequestMessage
          {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://shoes-collections.p.rapidapi.com/shoes"),
            Headers =
            {
              { "X-RapidAPI-Key", "77fc18d8a9msh3eee77069f0cf92p1d2fecjsn8d66d0d39fe2" },
              { "X-RapidAPI-Host", "shoes-collections.p.rapidapi.com" },
            },
          };
          using (var response = await client.SendAsync(request))
          {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(body);

            ViewBag.ImagenZapatilla = data[random.Next(1,30)].image;
            ViewBag.DescripcionZapatilla = data[random.Next(1,30)].description;
          }

          

          return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        
    }
}