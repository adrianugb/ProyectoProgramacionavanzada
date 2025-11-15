using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoFinal_G1_Autenticado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string MakeMeAdmin()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = userManager.FindByEmail("adriangbonilla49@gmail.com");

            if (user != null)
            {
                if (!userManager.IsInRole(user.Id, "Administrador"))
                {
                    userManager.AddToRole(user.Id, "Administrador");
                    return "✔ Ahora sos Administrador.";
                }
                return "Ya eras Administrador.";
            }

            return "✘ El usuario no existe. ¿Estás seguro del correo?";
        }
    }
}