using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    [Authorize(Roles = "Asociado")]
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var items = db.CartItems
                .Include(c => c.Product)
                .Include(c => c.Product.Images)
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.CartTotal = items.Sum(i => i.Product.Price * i.Quantity);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var userId = User.Identity.GetUserId();
            var item = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    db.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int productId)
        {
            var userId = User.Identity.GetUserId();
            var item = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            var userId = User.Identity.GetUserId();
            var items = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!items.Any())
            {
                return RedirectToAction("Index");
            }

            var errors = new List<string>();
            foreach (var item in items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    errors.Add($"'{item.Product.Name}' solo tiene {item.Product.Stock} unidades disponibles.");
                }
                if (item.Product.Status != ProductStatus.Activo)
                {
                    errors.Add($"'{item.Product.Name}' ya no está disponible.");
                }
            }

            if (errors.Any())
            {
                TempData["Error"] = string.Join(" ", errors);
                return RedirectToAction("Index");
            }

            ViewBag.CartTotal = items.Sum(i => i.Product.Price * i.Quantity);
            return View(items);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

