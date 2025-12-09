using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetByUserId(string userId);
        Order GetWithDetails(int orderId);
    }
}

