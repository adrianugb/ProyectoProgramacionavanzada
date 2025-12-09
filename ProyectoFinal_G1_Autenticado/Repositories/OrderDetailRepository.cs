using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository() : base() { }

        public OrderDetailRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<OrderDetail> GetByOrderId(int orderId)
        {
            return _dbSet.Include(d => d.Product).Where(d => d.OrderId == orderId).ToList();
        }
    }
}

