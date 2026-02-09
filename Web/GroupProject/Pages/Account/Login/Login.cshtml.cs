using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;

public class LoginModel : PageModel
{
    private readonly WebServiceClient client;

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public LoginModel(WebServiceClient client)
    {
        this.client = client;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = client.LoginAsync(Email, Password).Result;

        if (user == null)
        {
            TempData["ErrorMessage"] = "Invalid email or password.";
            return Page();
        }

        HttpContext.Session.SetInt32("USERID", user.userId);
        HttpContext.Session.SetString("ROLE", user.userType);

        return RedirectToPage("/Index");
    }
}