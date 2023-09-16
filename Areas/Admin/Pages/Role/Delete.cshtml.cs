using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppRazor.models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace AppRazor.Admin.Role
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, AppDBContext myBlogContext)
                    : base(roleManager, myBlogContext)
        {
        }

        public IdentityRole role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy vai trò");
                
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy vai trò");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid){
            if (roleid == null) return NotFound("Không tìm thấy vai trò");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("Không tìm thấy vai trò");

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa xóa vai trò: {role.Name}";
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
