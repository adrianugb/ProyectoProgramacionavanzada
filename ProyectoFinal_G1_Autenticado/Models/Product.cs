using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public enum ProductStatus
    {
        Inactivo = 0,
        Activo = 1
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Activo;

        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
