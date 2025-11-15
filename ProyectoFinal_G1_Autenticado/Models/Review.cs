using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public enum ReviewStatus
    {
        Pendiente = 0,
        Aprobada = 1,
        Rechazada = 2
    }

    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public string UserId { get; set; }

        [Required, StringLength(500)]
        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public ReviewStatus Status { get; set; } = ReviewStatus.Pendiente;
    }
}
