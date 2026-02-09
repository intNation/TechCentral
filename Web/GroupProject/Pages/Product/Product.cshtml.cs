using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System;

public class ProductModel : PageModel
{
    private readonly WebServiceClient client;

    public ProductModel(WebServiceClient client)
    {
        this.client = client;
    }

    public Product Product { get; set; }
    public string Layout { get; set; }

    public int id { get; set; }
    public string userType { get; set; }
    public IActionResult OnGet()
    {
        userType = HttpContext.Session.GetString("ROLE");
        int id = Convert.ToInt32(Request.Query["id"].ToString());

        //USER DEFINED ROLE FUNCTIONALITY
        //SWITCH LAYOUT BASED ON WHO IS LOGGED IN
        Layout = userType switch
        {
            "customer" => "_CustomerLayout",
            "admin" => "_AdminHomeLayout",
            _ => "_Layout",
        };

        //GET THE PRODUCT BY ID
        //HERE IN RAZOR PAGES WE DO NOT HAVE TO MANUALLY QUERY URL PARAMETER,
        // QUERIED AUTOMATICALLY WHEN THE ONGET METHOD IS CALLED
        Product = client.GetProductByID(id);

        //REDIRECT USER TO HOME IF PRODUCT IS NULL
        if (Product == null)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }


    public IActionResult OnPost(int productId, int quantity)
    {
        //GET THE USER ID STORED IN THE SESSION VARIABLE
        //WITH ASP.NET CORE, SESSION VARIABLE DATA ARE RETRIEVED USING THIS METHOD USING THE SESSION VARIABLE'S KEY
        int? userId = HttpContext.Session.GetInt32("USERID");

        //REDIRECT USER TO LOGIN IF ATTEMPTING TO ADD A PRODUCT TO CART WITHOUT LOGGIN IN
        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }

        //add to cart
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