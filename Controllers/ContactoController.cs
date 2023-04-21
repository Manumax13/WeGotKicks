using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeGotKicks.Models;
using WeGotKicks.Data;
 

namespace HeladeriaTAMS.Controllers
{
  
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;

//agregamos nueva sentencia
        private readonly ApplicationDbContext _context;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
             _context = context; //AGREGAMOS
        }

        public IActionResult Index()
        {
            return View("Index");
        }
 

            [HttpPost]
        public  IActionResult Create(Contacto objContacto)
        {
             Console.WriteLine("--------------------------------------------");
              Console.WriteLine("objContacto: " + objContacto.Name);

              if (objContacto != null){
                    _context.Add(objContacto);
                    _context.SaveChanges();
              }else{
                 Console.WriteLine("--------------------------------------------");
              Console.WriteLine("objContacto esta nullo: " );
              }

                    return View("VistaSubmit");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}