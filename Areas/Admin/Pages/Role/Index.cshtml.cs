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

        public class RoleModel : IdentityRole {
            public string[] Claims { get; set; }
        }
        public List<RoleModel> roles { get; set; }

        public async Task OnGetAsync()
        {
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel>();
            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + "=" + c.Value);

                var roleModel = new RoleModel(){
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimsString.ToArray()
                };
                roles.Add(roleModel);
            }
        }

        public IActionResult OnPost => RedirectToPage();

        public string? ClaimValue { get; internal set; }
        public string? ClaimType { get; internal set; }
    }
}
