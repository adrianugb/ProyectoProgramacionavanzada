using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository() : base() { }

        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Order> GetByUserId(string userId)
        {
            return _dbSet.Include(o => o.Details).Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList();
        }

        public Order GetWithDetails(int orderId)
        {
            return _dbSet.Include(o => o.Details.Select(d => d.Product)).FirstOrDefault(o => o.Id == orderId);
        }
    }
}

