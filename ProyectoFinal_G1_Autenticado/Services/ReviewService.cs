using System;
using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public class ReviewService : IReviewService, IDisposable
    {
        private IReviewRepository reviewRepo;

        public ReviewService()
        {
            reviewRepo = new ReviewRepository();
        }

        public IEnumerable<Review> GetProductReviews(int productId)
        {
            return reviewRepo.GetByProductId(productId);
        }

        public IEnumerable<Review> GetApprovedReviews(int productId)
        {
            return reviewRepo.GetApprovedByProductId(productId);
        }

        public IEnumerable<Review> GetPendingReviews()
        {
            return reviewRepo.GetPendingReviews();
        }

        public Review GetById(int id)
        {
            return reviewRepo.GetById(id);
        }

        public void CreateReview(Review review)
        {
            review.CreatedAt = DateTime.Now;
            review.Status = ReviewStatus.Pendiente;
            reviewRepo.Add(review);
            reviewRepo.Save();
        }

        public void ApproveReview(int id)
        {
            var review = reviewRepo.GetById(id);
            if (review != null)
            {
                review.Status = ReviewStatus.Aprobada;
                reviewRepo.Update(review);
                reviewRepo.Save();
            }
        }

        public void RejectReview(int id)
        {
            var review = reviewRepo.GetById(id);
            if (review != null)
            {
                review.Status = ReviewStatus.Rechazada;
                reviewRepo.Update(review);
                reviewRepo.Save();
            }
        }

        public void Dispose()
        {
            (reviewRepo as IDisposable)?.Dispose();
        }
    }
}

