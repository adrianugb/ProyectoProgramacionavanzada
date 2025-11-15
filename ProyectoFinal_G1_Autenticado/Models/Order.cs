using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public enum OrderStatus
    {
        Pendiente = 0,
        Procesando = 1,
        Completada = 2,
        Cancelada = 3
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }

        public virtual ICollection<OrderDetail> Details { get; set; }

        [NotMapped]
        public decimal CalculatedTotal =>
            Details?.Sum(d => d.Quantity * d.UnitPrice) ?? 0m;
    }
}
