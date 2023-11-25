using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Assignment.Controllers
{
    public class AdminController : Controller
    {
        NorthwindContext context = new NorthwindContext();
        public IActionResult Index()
        {
            var data = context.OrderDetails.Include(x => x.Product).GroupBy(x => x.Product.ProductName).Select(group => new
            {
                ProductId = group.Key,
                SumQuan = group.Sum(X => X.ProductId),
            }).ToList();
            ViewBag.quanProduct = context.Products.Count();
            ViewBag.quanSupplier = context.Suppliers.Count();
            ViewBag.quanOrder = context.Orders.Count();
            ViewBag.quanOrderComplete = context.Orders.Count(x => x.Status == 1);
            ViewBag.data = data;
            return View();
        }
        public IActionResult ViewProducts(string SearchString = "",int page =1)
        {
            var list = context.Products.Include(x => x.Category).Include(x => x.Supplier).Where(x => SearchString.Equals("") || x.ProductName.Contains(SearchString)).ToPagedList(page, 5);
            ViewBag.list = list;
            ViewBag.SearchString = SearchString;
            return View(list);
        }
        public IActionResult Search(string SearchString)
        {
            return RedirectToAction("ViewProducts", new {SearchString = SearchString});
        }
        public IActionResult Delete(int id)
        {
            List<OrderDetail> list = context.OrderDetails.Where(p => p.ProductId == id).ToList();
            Product product = context.Products.Find(id);
            context.OrderDetails.RemoveRange(list);
            context.Products.Remove(product);
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.product = context.Products.Find(id);
            ViewBag.categories = context.Categories.ToList();
            ViewBag.suppliers = context.Suppliers.ToList();
            return View();
        }
        public IActionResult DoEdit(Product p)
        {
            Product product = context.Products.Find(p.ProductId);
            product.ProductName = p.ProductName;
            product.SupplierId = p.SupplierId;
            product.CategoryId = p.CategoryId;
            product.QuantityPerUnit = p.QuantityPerUnit;
            product.UnitPrice = p.UnitPrice;
            context.SaveChanges();
            return RedirectToAction("ViewProducts");
        }
        public IActionResult Create()
        {
            ViewBag.categories = context.Categories.ToList();
            ViewBag.suppliers = context.Suppliers.ToList();
            return View();
        }

        public IActionResult DoCreate(Product p)
        {
            context.Products.Add(p);
            context.SaveChanges();
            return RedirectToAction("ViewProducts");
        }
        public IActionResult Tracking()
        {
            List<Order> order = context.Orders.ToList();
            ViewBag.order = order;
            return View();
        }
        public IActionResult Confirm(int id)
        {
            Order order = context.Orders.Find(id);
            order.Status = 3;
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
        public IActionResult Cancel(int id)
        {
            Order order = context.Orders.Find(id);
            order.Status = 4;
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
        public IActionResult NotCancel(int id)
        {
            Order order = context.Orders.Find(id);
            order.Status = 1;
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
    }
}
