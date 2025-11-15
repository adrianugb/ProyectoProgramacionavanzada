using System.Collections.Generic;
using System.Web.Mvc;

namespace ProyectoFinal_G1_Autenticado.Models
{
    public class EditRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string SelectedRole { get; set; }
        public List<SelectListItem> RolesList { get; set; }
    }
}
