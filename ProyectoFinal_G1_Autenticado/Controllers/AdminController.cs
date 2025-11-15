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

        // Lista usuarios
        public ActionResult Users()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // GET: Editar roles de un usuario
        public async Task<ActionResult> EditRoles(string id)
        {
            if (id == null) return HttpNotFound();

            var user = await userManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            var model = new EditRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                RolesList = roleManager.Roles
                                .Select(r => new SelectListItem
                                {
                                    Selected = userManager.IsInRole(user.Id, r.Name),
                                    Text = r.Name,
                                    Value = r.Name
                                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRoles(EditRolesViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null) return HttpNotFound();

            var userRoles = await userManager.GetRolesAsync(user.Id);

            // Remover roles existentes
            await userManager.RemoveFromRolesAsync(user.Id, userRoles.ToArray());

            if (model.SelectedRoles != null)
            {
                await userManager.AddToRolesAsync(user.Id, model.SelectedRoles);
            }

            return RedirectToAction("Users");
        }
    }
}
