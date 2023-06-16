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
              { "X-RapidAPI-Key", "4d32f0438cmsh870e5849edb358bp17c191jsn1833dd5078fe" },
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