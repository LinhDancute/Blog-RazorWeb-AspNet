using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using AppRazor.models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace AppRazor.Admin.Role
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, AppDBContext myBlogContext)
                    : base(roleManager, myBlogContext)
        {
        }

        public class InputModel {
            [Display(Name = "Tên vai trò")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự ")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(){
            if(!ModelState.IsValid) {
                return Page();
            }

            var newRole = new IdentityRole(Input.Name);
            var result =  await _roleManager.CreateAsync(newRole);
            
            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo vai trò mới: {Input.Name}";
                return RedirectToPage("./Index");
            } else
            {
                result.Errors.ToList().ForEach (error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                
            }

            return Page();
        }
    }
}
