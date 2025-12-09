using System.Collections.Generic;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository() : base() { }

        public ProductImageRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ProductImage> GetByProductId(int productId)
        {
            return _dbSet.Where(i => i.ProductId == productId).OrderBy(i => i.SortOrder).ToList();
        }
    }
}

