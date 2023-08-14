using System.ComponentModel;
using AppRazor.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace AppRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly MyBlogContext _myBlogContext;

    public IndexModel(ILogger<IndexModel> logger, MyBlogContext myBlogContext)
    {
        _logger = logger;
        _myBlogContext = myBlogContext;
    }

    public IActionResult OnPost()
    {
        var username = this.Request.Form["username"];
        var message = new MessagePage.Message();
        message.title = "Thông báo";
        message.htmlcontent = $"Cảm ơn {username} đã gửi thông tin";
        message.secondwait = 3;
        message.urlredirect = Url.Page("Privacy");

        // Pass the 'message' object to the ViewComponent
        return ViewComponent("MessagePage", message);
    }


    public void OnGet()
    {
        var posts = (from a in _myBlogContext.articles 
                    orderby a.Created descending
                    select a).ToList();
        ViewData["posts"] = posts;
    }
}
