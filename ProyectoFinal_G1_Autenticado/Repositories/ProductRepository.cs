using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository() : base() { }

        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Product> GetActiveProducts()
        {
            return _dbSet.Where(p => p.Status == ProductStatus.Activo).ToList();
        }

        public Product GetByCode(string code)
        {
            return _dbSet.FirstOrDefault(p => p.Code == code);
        }

        public Product GetWithImages(int id)
        {
            return _dbSet.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
        }
    }
}

