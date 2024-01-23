using JQueryDataTablesMVC.Data;
using JQueryDataTablesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core; // se debe instalar esta extension para que funcione IQueryable 
using System.Linq.Expressions;
using System.Reflection; 

namespace JQueryDataTablesMVC.Extencions
{
    public abstract class DataTablesExtencion<T> : Controller where T : class
    {
        #region Properties
        private readonly ApplicationDbContext _db;
        #endregion

        #region Constructor
        public DataTablesExtencion(ApplicationDbContext db)
        {
            _db = db;
        }
        #endregion

        #region FillData
        [HttpPost]
        public async Task<ActionResult> FillDataAsync()
        {
            try
            {
                //valores que regresa el datatable
                string draw = Request.Form["draw"];
                string id = Request.Form["Id"];
                string start = Request.Form["start"];
                string lenght = Request.Form["length"];
                string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
                string sortColumnDir = Request.Form["order[0][dir]"];
                string searchValue = Request.Form["search[value]"];
                
                string searchValue2 = Request.Form["columns[2][search][value]"];

                int pageSize = lenght != null ? Convert.ToInt32(lenght) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                IQueryable<T> query = _db.Set<T>();

                if (searchValue != "")
                {
                    query = ApplySearchFilter(query, searchValue);
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
        #endregion

        private IQueryable<T> ApplySearchFilter(IQueryable<T> query, string searchValue)
        {
            var properties = typeof(T).GetProperties();

            var parameter = Expression.Parameter(typeof(T));
            Expression finalExpression = null;

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (propertyType == typeof(string))
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var toStringMethod = propertyType.GetMethod("ToString", Type.EmptyTypes);
                    var toStringExpression = Expression.Call(propertyAccess, toStringMethod);

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var filterExpression = Expression.Call(toStringExpression, containsMethod, Expression.Constant(searchValue));

                    if (finalExpression == null)
                        finalExpression = filterExpression;
                    else
                        finalExpression = Expression.Or(finalExpression, filterExpression);
                }
                else if (propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null)
                {
                    var filterExpression = BuildValueFilterExpression(parameter, property, searchValue);

                    if (finalExpression == null)
                        finalExpression = filterExpression;
                    else
                        finalExpression = Expression.Or(finalExpression, filterExpression);
                }
            }

            if (finalExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private Expression BuildValueFilterExpression(ParameterExpression parameter, PropertyInfo property, string searchValue)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var toStringMethod = property.PropertyType.GetMethod("ToString", Type.EmptyTypes);
            var toStringExpression = Expression.Call(propertyAccess, toStringMethod);

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var filterExpression = Expression.Call(toStringExpression, containsMethod, Expression.Constant(searchValue));

            return filterExpression;
        }




    }
}
