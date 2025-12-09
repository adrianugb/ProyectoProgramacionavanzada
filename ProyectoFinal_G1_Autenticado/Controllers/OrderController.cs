using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderRepository orderRepo = new OrderRepository();
        private ICartItemRepository cartRepo = new CartItemRepository();
        private IProductRepository productRepo = new ProductRepository();

        [Authorize(Roles = "Asociado")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var orders = orderRepo.GetByUserId(userId).ToList();

            return View(orders);
        }

        [Authorize(Roles = "Asociado")]
        public ActionResult Details(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = orderRepo.GetWithDetails(id);

            if (order == null || order.UserId != userId)
                return HttpNotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Asociado")]
        public ActionResult ConfirmPurchase()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = cartRepo.GetByUserId(userId).ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Cart");
            }

            var errors = new List<string>();
            foreach (var item in cartItems)
            {
                var product = productRepo.GetById(item.ProductId);
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
                var product = productRepo.GetById(item.ProductId);

                var detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.Details.Add(detail);
                total += detail.Quantity * detail.UnitPrice;

                product.Stock -= item.Quantity;
                productRepo.Update(product);
            }

            order.Total = total;
            orderRepo.Add(order);
            orderRepo.Save();

            // Limpiar carrito
            foreach (var item in cartItems)
            {
                cartRepo.Delete(item);
            }
            cartRepo.Save();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        [Authorize(Roles = "Asociado")]
        public ActionResult Confirmation(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = orderRepo.GetWithDetails(id);

            if (order == null || order.UserId != userId)
                return HttpNotFound();

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (orderRepo as IDisposable)?.Dispose();
                (cartRepo as IDisposable)?.Dispose();
                (productRepo as IDisposable)?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
