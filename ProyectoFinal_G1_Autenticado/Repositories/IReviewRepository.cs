using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        IEnumerable<Review> GetByProductId(int productId);
        IEnumerable<Review> GetPendingReviews();
        IEnumerable<Review> GetApprovedByProductId(int productId);
    }
}

