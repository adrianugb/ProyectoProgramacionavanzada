using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public interface IReviewService
    {
        IEnumerable<Review> GetProductReviews(int productId);
        IEnumerable<Review> GetApprovedReviews(int productId);
        IEnumerable<Review> GetPendingReviews();
        Review GetById(int id);
        void CreateReview(Review review);
        void ApproveReview(int id);
        void RejectReview(int id);
    }
}

