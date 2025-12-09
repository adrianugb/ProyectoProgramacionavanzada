using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public interface IOrderService
    {
        IEnumerable<Order> GetUserOrders(string userId);
        Order GetOrderById(int orderId);
        Order GetOrderWithDetails(int orderId);
        Order CreateOrder(string userId);
        IEnumerable<Order> GetAllOrders();
        bool ValidateCartForCheckout(string userId, out List<string> errors);
    }
}

