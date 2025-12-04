using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/cart")]
    public class CartApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = db.Products.Find(request.ProductId);
            if (product == null || product.Status != ProductStatus.Activo)
                return BadRequest("Producto no disponible");

            var existingItem = db.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
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
                db.CartItems.Add(cartItem);
            }

            db.SaveChanges();

            var itemCount = db.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => (int?)c.Quantity) ?? 0;

            return Ok(new { success = true, cartItemCount = itemCount, message = "Producto agregado al carrito" });
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var item = db.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (item == null)
                return NotFound();

            if (request.Quantity <= 0)
            {
                db.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            db.SaveChanges();

            var total = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Sum(c => (decimal?)(c.Product.Price * c.Quantity)) ?? 0;

            var itemCount = db.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => (int?)c.Quantity) ?? 0;

            return Ok(new { success = true, cartTotal = total, cartItemCount = itemCount });
        }

        [HttpDelete]
        [Route("remove/{productId}")]
        public IHttpActionResult RemoveFromCart(int productId)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var item = db.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }

            var total = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Sum(c => (decimal?)(c.Product.Price * c.Quantity)) ?? 0;

            var itemCount = db.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => (int?)c.Quantity) ?? 0;

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

            var count = db.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => (int?)c.Quantity) ?? 0;

            return Ok(new { count = count });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
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

