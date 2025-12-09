using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/cart")]
    public class CartApiController : ApiController
    {
        private ICartItemRepository cartRepo = new CartItemRepository();
        private IProductRepository productRepo = new ProductRepository();

        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = productRepo.GetById(request.ProductId);
            if (product == null || product.Status != ProductStatus.Activo)
                return BadRequest("Producto no disponible");

            var existingItem = cartRepo.GetByUserAndProduct(userId, request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                cartRepo.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    AddedAt = DateTime.Now
                };
                cartRepo.Add(cartItem);
            }

            cartRepo.Save();

            var itemCount = cartRepo.GetByUserId(userId).Sum(c => c.Quantity);

            return Ok(new { success = true, cartItemCount = itemCount, message = "Producto agregado al carrito" });
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var item = cartRepo.GetByUserAndProduct(userId, request.ProductId);

            if (item == null)
                return NotFound();

            if (request.Quantity <= 0)
            {
                cartRepo.Delete(item);
            }
            else
            {
                item.Quantity = request.Quantity;
                cartRepo.Update(item);
            }

            cartRepo.Save();

            var items = cartRepo.GetByUserId(userId).ToList();
            var total = items.Sum(c => c.Product.Price * c.Quantity);
            var itemCount = items.Sum(c => c.Quantity);

            return Ok(new { success = true, cartTotal = total, cartItemCount = itemCount });
        }

        [HttpDelete]
        [Route("remove/{productId}")]
        public IHttpActionResult RemoveFromCart(int productId)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var item = cartRepo.GetByUserAndProduct(userId, productId);

            if (item != null)
            {
                cartRepo.Delete(item);
                cartRepo.Save();
            }

            var items = cartRepo.GetByUserId(userId).ToList();
            var total = items.Sum(c => c.Product.Price * c.Quantity);
            var itemCount = items.Sum(c => c.Quantity);

            return Ok(new { success = true, cartTotal = total, cartItemCount = itemCount, message = "Producto eliminado" });
        }

        [HttpGet]
        [Route("count")]
        [AllowAnonymous]
        public IHttpActionResult GetCartCount()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Ok(new { count = 0 });

            var count = cartRepo.GetByUserId(userId).Sum(c => c.Quantity);

            return Ok(new { count = count });
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

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
