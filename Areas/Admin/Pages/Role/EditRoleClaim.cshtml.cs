using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using AppRazor.models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AppRazor.Admin.Role
{
    [Authorize(Roles = "Administrator")]
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext)
                    : base(roleManager, myBlogContext)
        {
        }

        public class InputModel {
            [Display(Name = "Tên đặc tính (claim)")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự ")]
            public string ClaimType { get; set; }

            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự ")]
            public string ClaimValue { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole role { get; set; }
        IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy đặc tính");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy đặc tính");
            
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy vai trò");

            Input = new InputModel {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy đặc tính");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy đặc tính");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy vai trò");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.RoleClaims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có trong vai trò");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Vừa cập nhật đặc tính";
            return RedirectToPage("./Edit", new {roleid = role.Id});
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = "Vừa xóa đặc tính";
            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
