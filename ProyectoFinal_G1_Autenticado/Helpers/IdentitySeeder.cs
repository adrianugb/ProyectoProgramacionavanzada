using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Identity
{
    public class IdentitySeeder
    {
        public static void Seed()
        {
            using (var context = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (!roleManager.RoleExists("Administrador"))
                {
                    var role = new IdentityRole("Administrador");
                    roleManager.Create(role);
                }

                string adminEmail = "admin@tudominio.com";
                string adminPassword = "Admin123!";

                var adminUser = userManager.FindByEmail(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Administrador Principal",
                        IsActive = true
                    };
                    var result = userManager.Create(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        userManager.AddToRole(adminUser.Id, "Administrador");
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
