using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppRazor.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AppRazor.Admin.Role
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext)
            : base(roleManager, myBlogContext)
        {
        }

        public List<IdentityRole> roles { get; set; }

        public async Task OnGetAsync()
        {
            roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        public IActionResult OnPost => RedirectToPage();
    }
}
