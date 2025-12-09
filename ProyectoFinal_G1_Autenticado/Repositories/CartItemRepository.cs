using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository() : base() { }

        public CartItemRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<CartItem> GetByUserId(string userId)
        {
            return _dbSet.Include(c => c.Product).Where(c => c.UserId == userId).ToList();
        }

        public CartItem GetByUserAndProduct(string userId, int productId)
        {
            return _dbSet.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
        }
    }
}

