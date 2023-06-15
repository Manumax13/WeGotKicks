using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using WeGotKicks.Data;
using WeGotKicks.Models;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;
using OfficeOpenXml.Table;
using Rotativa.AspNetCore;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WeGotKicks.Controllers
{
   
    public class PagoController : Controller
    {
         private readonly ILogger<PagoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public PagoController(ILogger<PagoController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Create(Decimal monto)
        {
            Pago pago = new Pago();
            pago.UserID = _userManager.GetUserName(User);

            pago.MontoTotal = monto;
            //pago.MontoTotal =  Convert.ToDecimal(TempData["montoTotal"]);
            return View(pago);
        }


        [HttpPost]
        public IActionResult Pagar(Pago pago)
        {
            pago.PaymentDate = DateTime.UtcNow;
            _context.Add(pago);

            var itemsProforma = from o in _context.DataProforma select o;
            itemsProforma = itemsProforma.
                Include(p => p.Producto).
                Where(s => s.UserID.Equals(pago.UserID) && s.Status.Equals("PENDIENTE"));

            Pedido pedido = new Pedido();
            pedido.UserID = pago.UserID;
            pedido.Total = pago.MontoTotal;
            pedido.pago = pago;
            pedido.Status = "PENDIENTE";
            _context.Add(pedido);

     
            List<DetallePedido> itemsPedido = new List<DetallePedido>();
            foreach(var item in itemsProforma.ToList()){
                DetallePedido detallePedido = new DetallePedido();
                detallePedido.pedido=pedido;
                detallePedido.Precio = item.Precio;
                detallePedido.Producto = item.Producto;
                detallePedido.Cantidad = item.Cantidad;
                itemsPedido.Add(detallePedido);
            }

            _context.AddRange(itemsPedido);

            foreach (Proforma p in itemsProforma.ToList())
            {
                p.Status="PROCESADO";
            }
            _context.UpdateRange(itemsProforma);

            _context.SaveChanges();


            ViewData["Message"] = "El pago se ha registrado";

            TempData["PedidoId"] = pedido.ID;
            //   return View("Create");

           return RedirectToAction("RegistrarPagoSubmit");
        }
        
          //MOSTRAR EL DETALLE DEL PAGO
        public IActionResult RegistrarPagoSubmit(){

            int pedidoID =  (int)TempData["PedidoId"] ;


        Console.WriteLine("GENERAR LA VISTA" + " " + pedidoID);
            var productos = from o in _context.DataProductos select o;
            var order = from o in _context.DataPedido select o;
            var orderDetail = from o in _context.DataDetallePedido select o;
            
            var pagos = order.Where(s=> s.ID==pedidoID);
            var detaPedidos = orderDetail.Where(s=> s.pedido.ID==pedidoID);

            Console.WriteLine("---CONTEO PAGOS--------------------------------------------------");
            Console.WriteLine(pagos.ToList().Count.ToString());

            Console.WriteLine("---CONTEO- DETALLE-------------------------------------------------");
            Console.WriteLine(detaPedidos.ToList().Count.ToString());

            Console.WriteLine("---CONTEO PRODUCTOS--------------------------------------------------");
            Console.WriteLine(productos.ToList().Count.ToString());

            var pd = new Producto();
            var ListadoFiltroProd = new List<Producto>();
            
           /*  foreach (DetallePedido p in detaPedidos.ToList()){
              pd = (Producto)productos.Where(s => s.Id.Equals(p.Producto.Id)) ;
            
            Console.WriteLine("---NOMBRES---------------------------------------------------------");
            Console.WriteLine(pd.Name);
            }
            */

            //ListadoFiltroProd = productos.Where(s => detaPedidos.ToList().Any(f => f.Producto.Id == s.Id)).ToList();
            int[] idList = new int[detaPedidos.ToList().Count];
            
            for(int i=0; i<detaPedidos.ToList().Count; i++){
                  idList[i] = detaPedidos.ToList()[i].Producto.Id;
                  //GUARDA LOS IDS DEL DETALLE DEL PEDIDO

                 Console.WriteLine("---ID DETALLE PEDIDO---------------------------------------------------------");
            Console.WriteLine(idList[i]);

            }
            productos = productos.Where(s => idList.Contains(s.Id));
            //FILTRA EL ID DEL PEDIDO VS CON LOS QUE ESTAN EN LA BD

          return View(productos);
        }

        public IActionResult Index()
        {
            return View(_context.DataPago.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
         public IActionResult ExportarExcel()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var pagos = _context.DataPago.AsNoTracking().ToList();
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Pagos");
                worksheet.Cells["A1"].LoadFromCollection(pagos, PrintHeaders: true);
                for (var col = 1; col < pagos.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
                // Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: pagos.Count + 1, toColumn: 2), "Pagos");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;

                return File(libro.GetAsByteArray(), excelContentType, "Pagos.xlsx");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PruebaApi(string selectedValue,int montoTotal){

            var urlPrueba= "https://currency-converter-exchange-rates1.p.rapidapi.com/convert?amount="+montoTotal+".00&from=PEN&to="+"USD";
            Console.WriteLine(urlPrueba);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(urlPrueba),
                Headers =
                {
                    { "X-RapidAPI-Key", "79d5f81580mshe9ae6bcce5e7f46p1494d1jsn2a995efb82a5" },
                    { "X-RapidAPI-Host", "currency-converter-exchange-rates1.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(body);
                Console.WriteLine(json);
                ViewBag.montoTransformado=json.result;
            }
            Console.WriteLine("hola");

            return View("Create");
        }
    }
}