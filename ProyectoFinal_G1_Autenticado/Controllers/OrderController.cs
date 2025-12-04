using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Asociado")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var orders = db.Orders
                .Include(o => o.Details)
                .Include(o => o.Details.Select(d => d.Product))
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Asociado")]
        public ActionResult Details(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = db.Orders
                .Include(o => o.Details)
                .Include(o => o.Details.Select(d => d.Product))
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Asociado")]
        public ActionResult ConfirmPurchase()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Cart");
            }

            var errors = new List<string>();
            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    errors.Add($"'{product.Name}' solo tiene {product.Stock} unidades disponibles.");
                }
                if (product.Status != ProductStatus.Activo)
                {
                    errors.Add($"'{product.Name}' ya no está disponible.");
                }
            }

            if (errors.Any())
            {
                TempData["Error"] = string.Join(" ", errors);
                return RedirectToAction("Checkout", "Cart");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Completada,
                Details = new List<OrderDetail>()
            };

            decimal total = 0;

            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.ProductId);

                var detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.Details.Add(detail);
                total += detail.Quantity * detail.UnitPrice;

                product.Stock -= item.Quantity;
            }

            order.Total = total;
            db.Orders.Add(order);

            var itemsToRemove = db.CartItems.Where(c => c.UserId == userId).ToList();
            foreach (var item in itemsToRemove)
            {
                db.CartItems.Remove(item);
            }

            db.SaveChanges();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        [Authorize(Roles = "Asociado")]
        public ActionResult Confirmation(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = db.Orders
                .Include(o => o.Details)
                .Include(o => o.Details.Select(d => d.Product))
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

