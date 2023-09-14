using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AppRazor.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppRazor.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet() => NotFound("Không được truy cập");

        public class InputModel
        {
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
        public InputModel Input { set; get; }

        public AppUser user { get; set; }
        public IdentityUserClaim<string> userclaim { get; set; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy người dùng");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy người dùng");
            
            if(!ModelState.IsValid) return Page();
            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);

            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue)) {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho người dùng";
            return RedirectToPage("./AddRole", new { Id = user.Id });
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy người dùng");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if (user == null) return NotFound("Không tìm thấy người dùng");

            Input = new InputModel() {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy người dùng");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if (user == null) return NotFound("Không tìm thấy người dùng");

            if (!ModelState.IsValid) return Page();

            if (_context.UserClaims.Any(c => c.UserId == user.Id
                && c.ClaimType == Input.ClaimType
                && c.ClaimValue == Input.ClaimValue
                && c.Id != userclaim.Id))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            userclaim.ClaimType = Input.ClaimType;
            userclaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa cập nhật đặc tính";

            return RedirectToPage("./AddRole", new { Id = user.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy người dùng");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if (user == null) return NotFound("Không tìm thấy người dùng");

            await _userManager.RemoveClaimAsync(user, new Claim(userclaim.ClaimType, userclaim.ClaimValue));

            StatusMessage = "Bạn đã xóa đặc tính";

            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
    }
}
