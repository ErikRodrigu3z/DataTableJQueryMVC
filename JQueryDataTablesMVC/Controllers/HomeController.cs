using JQueryDataTablesMVC.Data;
using JQueryDataTablesMVC.Extencions;
using JQueryDataTablesMVC.Models;
using JQueryDataTablesMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JQueryDataTablesMVC.Controllers
{
    public class HomeController : Controller // DataTablesExtencion<Personas>
    {
        private readonly ILogger<HomeController> _logger;               
        private readonly DataTablesService<Personas> _service;

        public HomeController(ILogger<HomeController> logger, DataTablesService<Personas> service) //  : base(db)
        {
            _service = service;
            _logger = logger;            
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> FillDataAsync()
        {
            try
            {                
                return Json(await _service.FillDataAsync(Request));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
