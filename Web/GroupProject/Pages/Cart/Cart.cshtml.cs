using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

public class CartModel : PageModel
{
    private readonly WebServiceClient client;

    public CartModel(WebServiceClient client)
    {
        this.client = client;
    }

    public Cart UserCart { get; set; }
    public List<CartItem> CartItems { get; set; }
    public List<Product> Products { get; set; }
    public string Layout { get; set; }

    // Cart Summary
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public IActionResult OnGet()
    {
        string userType = HttpContext.Session.GetString("ROLE");
        int? userId = HttpContext.Session.GetInt32("USERID");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }

        Layout = userType switch
        {
            "customer" => "_CustomerLayout",
            "admin" => "_AdminHomeLayout",
            _ => "_Layout",
        };

        UserCart = client.GetCart(userId.Value);

        if (UserCart == null)
        {
            return Page();
        }

        CartItems = client.GetCartItems(UserCart.CartId);

        if (CartItems == null || !CartItems.Any())
        {
            return Page();
        }

        var productIds = CartItems.Select(ci => ci.ProductId).ToList();
        Products = client.GetProductsByIDs(productIds);

        Subtotal = CartItems.Sum(item =>
        {
            var product = Products.FirstOrDefault(p => p.productId == item.ProductId);
            var price = product.onSale && product.SalePrice.HasValue ? product.SalePrice.Value : product.Price;
            return price * item.QtyAdded;
        });

        Tax = Subtotal * 0.15m;
        Total = Subtotal + Tax;

        return Page();
    }

    public IActionResult OnPostRemove(int productId)
    {
        int? userId = HttpContext.Session.GetInt32("USERID");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }

        var cart = client.GetCart(userId.Value);
        if (cart != null)
        {
            var result = client.RemoveFromCart(cart.CartId, productId);
            if (result)
            {
                TempData["SuccessMessage"] = "Product removed from cart.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to remove product from cart.";
            }
        }

        return RedirectToPage();
    }
}