using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public class CartService : ICartService, IDisposable
    {
        private ICartItemRepository cartRepo;
        private IProductRepository productRepo;

        public CartService()
        {
            cartRepo = new CartItemRepository();
            productRepo = new ProductRepository();
        }

        public IEnumerable<CartItem> GetUserCart(string userId)
        {
            return cartRepo.GetByUserId(userId);
        }

        public CartItem GetCartItem(string userId, int productId)
        {
            return cartRepo.GetByUserAndProduct(userId, productId);
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var product = productRepo.GetById(productId);
            if (product == null || product.Status != ProductStatus.Activo)
                return;

            var existingItem = cartRepo.GetByUserAndProduct(userId, productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                cartRepo.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                cartRepo.Add(cartItem);
            }
            cartRepo.Save();
        }

        public void UpdateQuantity(string userId, int productId, int quantity)
        {
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
        }

        public void RemoveItem(string userId, int productId)
        {
            var item = cartRepo.GetByUserAndProduct(userId, productId);
            if (item != null)
            {
                cartRepo.Delete(item);
                cartRepo.Save();
            }
        }

        public void ClearCart(string userId)
        {
            var items = cartRepo.GetByUserId(userId);
            foreach (var item in items)
            {
                cartRepo.Delete(item);
            }
            cartRepo.Save();
        }

        public decimal GetCartTotal(string userId)
        {
            var items = cartRepo.GetByUserId(userId);
            return items.Sum(i => i.Product.Price * i.Quantity);
        }

        public int GetCartItemCount(string userId)
        {
            var items = cartRepo.GetByUserId(userId);
            return items.Sum(i => i.Quantity);
        }

        public void Dispose()
        {
            (cartRepo as IDisposable)?.Dispose();
            (productRepo as IDisposable)?.Dispose();
        }
    }
}

