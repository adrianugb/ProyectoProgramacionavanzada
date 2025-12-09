using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    [Authorize(Roles = "Asociado")]
    public class CartController : Controller
    {
        private ICartItemRepository cartRepo = new CartItemRepository();
        private IProductRepository productRepo = new ProductRepository();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var items = cartRepo.GetByUserId(userId).ToList();

            ViewBag.CartTotal = items.Sum(i => i.Product.Price * i.Quantity);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var userId = User.Identity.GetUserId();
            var item = cartRepo.GetByUserAndProduct(userId, productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cartRepo.Delete(item);
                }
                else
                {
                    item.Quantity = quantity;
                    cartRepo.Update(item);
                }
                cartRepo.Save();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int productId)
        {
            var userId = User.Identity.GetUserId();
            var item = cartRepo.GetByUserAndProduct(userId, productId);

            if (item != null)
            {
                cartRepo.Delete(item);
                cartRepo.Save();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            var userId = User.Identity.GetUserId();
            var items = cartRepo.GetByUserId(userId).ToList();

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
            if (disposing)
            {
                (cartRepo as IDisposable)?.Dispose();
                (productRepo as IDisposable)?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
