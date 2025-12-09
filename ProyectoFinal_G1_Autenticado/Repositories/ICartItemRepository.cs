using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        IEnumerable<CartItem> GetByUserId(string userId);
        CartItem GetByUserAndProduct(string userId, int productId);
    }
}

