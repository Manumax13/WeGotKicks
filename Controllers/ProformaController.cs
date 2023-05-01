
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeGotKicks.Data;
using WeGotKicks.Models;
using Microsoft.AspNetCore.Http;
/// <summary>
/// Aka Carrito
/// </summary>

namespace WeGotKicks.Controllers
{
    public class ProformaController : Controller
    {
        private readonly ILogger<ProformaController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public ProformaController(ApplicationDbContext context,
            ILogger<ProformaController> logger,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userIDSession = _userManager.GetUserName(User);
            var items = from o in _context.DataProforma select o;
            items = items.Include(p => p.Producto).
                    Where(w => w.UserID.Equals(userIDSession) &&
                    w.Status.Equals("PENDIENTE"));
            var itemsCarrito = items.ToList();
           var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);

           //decimal total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);
           
            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;

           // TempData["montoTotal"] =total.ToString();
            return View(model);
        }

  //EDITAR

  public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proforma = await _context.DataProforma.FindAsync(id);
            if (proforma == null)
            {
                return NotFound();
            }
            return View(proforma);
        }
      //ELIMINAR
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proforma = await _context.DataProforma.FindAsync(id);
            _context.DataProforma.Remove(proforma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

//SE GUARDAN LOS DATOS
[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cantidad,Precio,UserID")] Proforma proforma)
        {
            if (id != proforma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proforma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProformaExists(proforma.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(proforma);
        }
        private bool ProformaExists(int id)
        {
            return _context.DataProforma.Any(e => e.Id == id);
        }
    }



      
}