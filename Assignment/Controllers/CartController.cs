using Assignment.Models;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assignment.Controllers
{
    public class CartController : Controller
    {
        NorthwindContext context = new NorthwindContext();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Add(int id)
        {
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(jsonString))
            {
                var cart = JsonConvert.DeserializeObject<Dictionary<int ,int >>(jsonString);
                cart.Add(id, 1);
                var jsonstring = JsonConvert.SerializeObject(cart);
                _httpContextAccessor.HttpContext.Session.SetString("Cart", jsonstring);
            }
            else
            {
                var cart = new Dictionary<int, int>();
                cart.Add(id, 1);
                var jsonstring = JsonConvert.SerializeObject(cart);
                _httpContextAccessor.HttpContext.Session.SetString("Cart", jsonstring);
            }
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }

        public IActionResult ViewCart()
        {
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            Dictionary<int, int> cart = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(jsonString))
            {
                cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            }
            ViewBag.Cart = cart;
            ViewBag.Quantity = cart.Count;
            return View();
        }
        public IActionResult DecreaseProduct(int id)
        {
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            if (cart[id] == 1)
            {
                cart.Remove(id);
            }
            else
            {
                cart[id]--;
            }
            var jsonstring = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Session.SetString("Cart", jsonstring);
            return RedirectToAction("ViewCart");
        }
        public IActionResult IncreaseProduct(int id)
        {
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            cart[id]++;
            var jsonstring = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Session.SetString("Cart", jsonstring);
            return RedirectToAction("ViewCart");
        }
        public IActionResult RemoveProduct(int id)
        {
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            cart.Remove(id);
            var jsonstring = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Session.SetString("Cart", jsonstring);
            return RedirectToAction("ViewCart");
        }
        public IActionResult AddOrder()
        {
            Order order = new Order();
            order.CustomerId = HttpContext.Session.GetString("Username");
            order.OrderDate = DateTime.Now;
            order.Status = 2;
            context.Orders.Add(order);
            context.SaveChanges();
            var jsonString = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            OrderDetail orderDetail = new OrderDetail();
            foreach(var item in cart)
            {
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductId = item.Key;
                orderDetail.Quantity = (short)item.Value;
                orderDetail.UnitPrice = (decimal)context.Products.Find(item.Key).UnitPrice;
                context.OrderDetails.Add(orderDetail);
                context.SaveChanges();
            }
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Tracking","Cart");
        }
        public IActionResult Tracking()
        {
            List<Order> order = context.Orders.Where(x => x.CustomerId == HttpContext.Session.GetString("Username")).ToList();
            ViewBag.order = order;
            return View();
        }

        public IActionResult Confirm(int id)
        {
            Order order = context.Orders.Find(id);
            order.Status = 1;
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
        public IActionResult Cancel(int id)
        {
            Order order = context.Orders.Find(id);
            order.Status = 5;
            context.SaveChanges();
            string refererUri = Request.Headers.Referer;
            return Redirect(refererUri);
        }
    }
}
