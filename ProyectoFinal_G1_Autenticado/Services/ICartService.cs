using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetUserCart(string userId);
        CartItem GetCartItem(string userId, int productId);
        void AddToCart(string userId, int productId, int quantity);
        void UpdateQuantity(string userId, int productId, int quantity);
        void RemoveItem(string userId, int productId);
        void ClearCart(string userId);
        decimal GetCartTotal(string userId);
        int GetCartItemCount(string userId);
    }
}

