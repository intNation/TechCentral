using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

public class CategoryProductsModel : PageModel
{
    private readonly WebServiceClient client;

    public CategoryProductsModel(WebServiceClient client)
    {
        this.client = client;
    }

    public List<Product> Products { get; set; }
    public string Category { get; set; }
    public string Layout { get; set; }
    public string userType { get; set; }

    public void OnGet(string category)
    {
        userType = HttpContext.Session.GetString("ROLE");

        Layout = userType switch
        {
            "customer" => "_CustomerLayout",
            "admin" => "_AdminHomeLayout",
            _ => "_Layout",
        };

        Category = category;
        Products = client.GetProductsByCategory(category);
    }

    public IActionResult OnPost(int productId, int quantity)
    {
        int? userId = HttpContext.Session.GetInt32("USERID");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }

        var result = client.AddToCart(userId.Value, productId, quantity);

        if (result)
        {
            TempData["SuccessMessage"] = "Product added to cart successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to add product to cart.";
        }

        return RedirectToPage("/Cart/Cart");
    }
}