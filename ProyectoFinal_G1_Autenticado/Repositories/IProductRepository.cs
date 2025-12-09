using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetActiveProducts();
        Product GetByCode(string code);
        Product GetWithImages(int id);
    }
}

