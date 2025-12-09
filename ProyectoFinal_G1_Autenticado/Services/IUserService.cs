using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetAllUsers();
        ApplicationUser GetById(string userId);
        void UpdateLastLogin(string userId);
        void ToggleUserStatus(string userId);
        int GetTotalUsersCount();
        int GetActiveUsersCount();
    }
}

