using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetActiveProducts();
        Product GetById(int id);
        Product GetWithImages(int id);
        Product GetByCode(string code);
        void Create(Product product);
        void Update(Product product);
        void Delete(int id);
        void ToggleStatus(int id);
    }
}

