using DataTableJQueryMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic.Core; // se debe instalar esta extension para que funcione IQueryable

namespace DataTableJQueryMVC.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ApplicationDbContext _db;

        //datatable properties
        //public string draw = "";
        //public string start = ""; 
        //public string lenght = ""; 
        //public string sortColumn = ""; 
        //public string sortColumnDir = "";
        //public string searchValue = "";
        //public int pageSize, skip, recordsTotal;
        


        public PersonasController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult FillData()
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
                
                IQueryable<Models.Personas> query = _db.Personas;

                if (searchValue != "")
                {
                    query = query.Where(x => x.Nombre.Contains(searchValue));
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = query.Count();
                var list = query.Skip(skip).Take(pageSize).ToList();

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




    }
}
