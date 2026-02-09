using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

public class RegisterModel : PageModel
{
    private readonly WebServiceClient client;

    [BindProperty]
    public User user { get; set; }

    public RegisterModel(WebServiceClient client)
    {
        this.client = client;
    }

    public void OnGet()
    {
        user = new User();
       
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        user.userType = "customer";
        user.RegistrationDate = System.DateTime.Today;
        var registered = client.Register(user);

        if (!registered)
        {
            TempData["ErrorMessage"] = "Registration failed. Email might already be in use.";
            return Page();
        }

        return RedirectToPage("/Account/Login/Login");
    }
}