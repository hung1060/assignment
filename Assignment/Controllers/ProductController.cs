using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;


namespace Assignment.Controllers
{
    public class ProductController : Controller
    {
        NorthwindContext context = new NorthwindContext();
        public IActionResult List(int categoryid=0, int supplierid=0, int flowrange=0, int page = 1)
        {
            dynamic list;
            if (flowrange != 0)
            {
                list= (flowrange==1? context.Products.Include(x => x.Category).Include(x => x.Supplier).Where(x => categoryid == 0 || x.CategoryId == categoryid).Where(x => supplierid == 0 || x.SupplierId == supplierid).OrderBy(e => e.UnitPrice).ToPagedList(page, 4): context.Products.Include(x => x.Category).Include(x => x.Supplier).Where(x => categoryid == 0 || x.CategoryId == categoryid).Where(x => supplierid == 0 || x.SupplierId == supplierid).OrderByDescending(e => e.UnitPrice).ToPagedList(page, 4));
            }
            else
            {
                list = context.Products.Include(x => x.Category).Include(x => x.Supplier).Where(x => categoryid == 0 || x.CategoryId == categoryid).Where(x => supplierid == 0 || x.SupplierId == supplierid).ToPagedList(page, 4);
            }
            ViewBag.list = list;
            ViewBag.categories  = context.Categories.ToList();
            ViewBag.suppliers = context.Suppliers.ToList();
            ViewBag.flowrange = flowrange;
            ViewBag.categoryid = categoryid;
            ViewBag.supplierid = supplierid;
            return View(list);
        }

        public IActionResult Filter(int categoryid, int supplierid, int flowrange, int page = 1)
        {
            return RedirectToAction("List", new {categoryid = categoryid, supplierid = supplierid, flowrange = flowrange});
        }
    }
}
