
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ContactRequestModel : PageModel {
    [BindProperty]
    public UserContact userContact {set; get;}

    private readonly ILogger<ContactRequestModel> _logger;
    public ContactRequestModel(ILogger<ContactRequestModel> logger){
        _logger = logger;
        _logger.LogInformation("Init contact");
    }

    public double Tong(double a, double b)
    {
        return a + b;
    }

    public void OnPost(){
        Console.WriteLine(this.userContact.Email);
    }
}