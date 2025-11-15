using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public byte[] ImageData { get; set; }

        [StringLength(200)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; }

        public int SortOrder { get; set; }
    }
}
