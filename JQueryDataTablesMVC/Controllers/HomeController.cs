using JQueryDataTablesMVC.Data;
using JQueryDataTablesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Dynamic.Core; // se debe instalar esta extension para que funcione IQueryable 

namespace JQueryDataTablesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> FillData()
        {
            try
            {
                //valores que regresa el datatable
                string draw = Request.Form["draw"];
                string start = Request.Form["start"];
                string lenght = Request.Form["length"];
                string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
                string sortColumnDir = Request.Form["order[0][dir]"];
                string searchValue = Request.Form["search[value]"];

                int pageSize = lenght != null ? Convert.ToInt32(lenght) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                IQueryable<Personas> query = _db.Personas;

                if (searchValue != "")
                {
                    query = query.Where(x => x.Nombre.Contains(searchValue));
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = query.Count();
                var list = await query.Skip(skip).Take(pageSize).ToListAsync();

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = list
                });
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
