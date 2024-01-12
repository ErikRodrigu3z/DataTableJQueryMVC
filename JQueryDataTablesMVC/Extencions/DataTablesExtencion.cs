using JQueryDataTablesMVC.Data;
using JQueryDataTablesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // se debe instalar esta extension para que funcione IQueryable 

namespace JQueryDataTablesMVC.Extencions
{
    public abstract class DataTablesExtencion : Controller
    {
        private readonly ApplicationDbContext _db;

        public DataTablesExtencion(ApplicationDbContext db)
        {
            _db = db;
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
    }
}
