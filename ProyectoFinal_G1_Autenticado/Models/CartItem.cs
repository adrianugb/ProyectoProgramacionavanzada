using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
