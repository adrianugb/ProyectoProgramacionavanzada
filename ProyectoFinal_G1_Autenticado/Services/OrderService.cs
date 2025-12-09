using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public class OrderService : IOrderService, IDisposable
    {
        private IOrderRepository orderRepo;
        private ICartItemRepository cartRepo;
        private IProductRepository productRepo;

        public OrderService()
        {
            orderRepo = new OrderRepository();
            cartRepo = new CartItemRepository();
            productRepo = new ProductRepository();
        }

        public IEnumerable<Order> GetUserOrders(string userId)
        {
            return orderRepo.GetByUserId(userId);
        }

        public Order GetOrderById(int orderId)
        {
            return orderRepo.GetById(orderId);
        }

        public Order GetOrderWithDetails(int orderId)
        {
            return orderRepo.GetWithDetails(orderId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return orderRepo.GetAll();
        }

        public bool ValidateCartForCheckout(string userId, out List<string> errors)
        {
            errors = new List<string>();
            var cartItems = cartRepo.GetByUserId(userId).ToList();

            if (!cartItems.Any())
            {
                errors.Add("El carrito está vacío.");
                return false;
            }

            foreach (var item in cartItems)
            {
                var product = productRepo.GetById(item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    errors.Add($"'{product.Name}' solo tiene {product.Stock} unidades disponibles.");
                }
                if (product.Status != ProductStatus.Activo)
                {
                    errors.Add($"'{product.Name}' ya no está disponible.");
                }
            }

            return !errors.Any();
        }

        public Order CreateOrder(string userId)
        {
            var cartItems = cartRepo.GetByUserId(userId).ToList();

            if (!cartItems.Any())
                return null;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Completada,
                Details = new List<OrderDetail>()
            };

            decimal total = 0;

            foreach (var item in cartItems)
            {
                var product = productRepo.GetById(item.ProductId);

                var detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.Details.Add(detail);
                total += detail.Quantity * detail.UnitPrice;

                // Actualizar stock
                product.Stock -= item.Quantity;
                productRepo.Update(product);
            }

            order.Total = total;
            orderRepo.Add(order);
            orderRepo.Save();
            productRepo.Save();

            // Limpiar carrito
            foreach (var item in cartItems)
            {
                cartRepo.Delete(item);
            }
            cartRepo.Save();

            return order;
        }

        public void Dispose()
        {
            (orderRepo as IDisposable)?.Dispose();
            (cartRepo as IDisposable)?.Dispose();
            (productRepo as IDisposable)?.Dispose();
        }
    }
}

