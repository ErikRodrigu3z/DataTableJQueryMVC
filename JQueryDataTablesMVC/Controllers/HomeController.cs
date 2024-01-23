using JQueryDataTablesMVC.Data;
using JQueryDataTablesMVC.Extencions;
using JQueryDataTablesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JQueryDataTablesMVC.Controllers
{
    public class HomeController : DataTablesExtencion<Personas>
    {
        private readonly ILogger<HomeController> _logger;       

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db) : base(db)
        {            
            _logger = logger;            
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
