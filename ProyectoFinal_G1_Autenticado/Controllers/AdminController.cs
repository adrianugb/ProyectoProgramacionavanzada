using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoFinal_G1_Autenticado.Models;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public AdminController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        // LISTA DE USUARIOS (vista principal)
        public ActionResult ListUsers()
        {
            var users = db.Users.ToList();

            var model = users.Select(u => new EditRolesViewModel
            {
                UserId = u.Id,
                Email = u.Email,
                SelectedRole = userManager.GetRoles(u.Id).FirstOrDefault(),
                RolesList = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList()
            }).ToList();

            return View(model);
        }

        //GET: Edit roles de un usuario
        public ActionResult EditRoles(string id)
        {
            if (id == null) return HttpNotFound();

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            var userRole = userManager.GetRoles(user.Id).FirstOrDefault();

            var allRoles = roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

            var model = new EditRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                RolesList = allRoles,
                SelectedRole = userRole
            };

            return View(model);
        }

        //POST: Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoles(EditRolesViewModel model)
        {
            var user = db.Users.Find(model.UserId);
            if (user == null) return HttpNotFound();

            var currentRoles = userManager.GetRoles(user.Id).ToList();

            // Quitar todos los roles actuales
            foreach (var role in currentRoles)
            {
                userManager.RemoveFromRole(user.Id, role);
            }

            // Agregar el nuevo rol seleccionado
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                userManager.AddToRole(user.Id, model.SelectedRole);
            }

            return RedirectToAction("ListUsers");
        }

    }
}
