using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ServiceReference1;
using System;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    private readonly WebServiceClient client;

    public IndexModel(WebServiceClient client)
    {
        this.client = client;
    }

    public List<Product> FeaturedProducts { get; set; }
    public List<Product> SaleItems { get; set; }
    public string Layout { get; set; }

    public string userType { get; set; }

    public void OnGet()
    {
         userType = HttpContext.Session.GetString("ROLE");

        //USER DEFINED ROLE FUNCTIONALITY
        //SWITCH LAYOUT BASED ON WHO IS LOGGED IN

        Layout = userType switch
        {
            "customer" => "_CustomerLayout",
            "admin" => "_AdminHomeLayout",
            _ => "_Layout",
        };

        FeaturedProducts = client.GetFeaturedProducts();
        SaleItems = client.GetProductsOnSaleByCategory("Accessories");
    }

    public IActionResult OnPost(int productId, int quantity)
    {
        int? userId = HttpContext.Session.GetInt32("USERID");


        //REDIRECT USER TO LOGIN IF ATTEMPTING TO ADD A PRODUCT TO CART WITHOUT LOGGIN IN

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }
        //ADDING TO CART
        var result = client.AddToCart(userId.Value, productId, quantity);

        //ADD TO CART RESPONSES
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