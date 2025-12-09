using System.Collections.Generic;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository() : base() { }

        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Review> GetByProductId(int productId)
        {
            return _dbSet.Where(r => r.ProductId == productId).ToList();
        }

        public IEnumerable<Review> GetPendingReviews()
        {
            return _dbSet.Where(r => r.Status == ReviewStatus.Pendiente).ToList();
        }

        public IEnumerable<Review> GetApprovedByProductId(int productId)
        {
            return _dbSet.Where(r => r.ProductId == productId && r.Status == ReviewStatus.Aprobada).ToList();
        }
    }
}

