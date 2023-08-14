using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class UserContact {
    [BindProperty(SupportsGet = true)]
    [DisplayName("ID của bạn")]
    [Range(10, 100, ErrorMessage = "Nhập sai")]
    public string UserID { set; get; }

    [BindProperty(SupportsGet = true)]
    [DisplayName("Email của bạn")]
    [EmailAddress(ErrorMessage = "Email sai định dạng")]
    public string Email { set; get; }

    [BindProperty(SupportsGet = true)]
    [DisplayName("Tên người dùng")]
    public string UserName { set; get; }
}