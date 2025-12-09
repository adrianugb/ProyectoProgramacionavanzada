using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public class UserService : IUserService, IDisposable
    {
        private ApplicationDbContext db;

        public UserService()
        {
            db = new ApplicationDbContext();
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return db.Users.ToList();
        }

        public ApplicationUser GetById(string userId)
        {
            return db.Users.Find(userId);
        }

        public void UpdateLastLogin(string userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        public void ToggleUserStatus(string userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                db.SaveChanges();
            }
        }

        public int GetTotalUsersCount()
        {
            return db.Users.Count();
        }

        public int GetActiveUsersCount()
        {
            return db.Users.Count(u => u.IsActive);
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}

