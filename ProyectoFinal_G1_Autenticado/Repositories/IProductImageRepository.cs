using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        IEnumerable<ProductImage> GetByProductId(int productId);
    }
}

