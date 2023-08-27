using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppRazor.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace AppRazor.Admin.User
{
    [Authorize(Roles = "Administrator")]
    //[Authorize(Roles = "Editor")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public class UserAndRole : AppUser {
            public string RoleNames { get; set; }
        }
        public List<UserAndRole> users { get; set; }
        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPages { get; set; }

        public int totalUsers { get; set; }
        public async Task OnGetAsync()
        {
            //users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);

            totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);

            if (currentPage < 1)
                currentPage = 1;

            if (currentPage > countPages)
                currentPage = countPages;

            var query = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).
                           Take(ITEMS_PER_PAGE)
                           .Select(u => new UserAndRole(){
                            Id = u.Id,
                            UserName = u.UserName,
                           });

            users = await query.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }

        public IActionResult OnPost => RedirectToPage();
    }
}
