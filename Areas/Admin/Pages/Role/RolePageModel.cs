using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppRazor.models;

namespace AppRazor.Admin.Role {
    public class RolePageModel : PageModel {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly AppDBContext _context;
        

        [TempData]
        public string StatusMessage { get; set; }
        public RolePageModel(RoleManager<IdentityRole> roleManager, AppDBContext myBlogContext)
        {
            _roleManager = roleManager;
            _context = myBlogContext;
        }
    }
}