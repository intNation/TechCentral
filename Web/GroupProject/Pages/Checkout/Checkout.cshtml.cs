using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

public class CheckoutModel : PageModel
{
    private readonly WebServiceClient client;

    public CheckoutModel(WebServiceClient client)
    {
        this.client = client;
    }

    public Cart UserCart { get; set; }
    public List<CartItem> CartItems { get; set; }
    public List<Product> Products { get; set; }
    public string Layout { get; set; }

    // Cart Summary
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
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToPage("/Cart/Cart");
        }

        CartItems = client.GetCartItems(UserCart.CartId);

        if (CartItems == null || !CartItems.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToPage("/Cart/Cart");
        }

        var productIds = CartItems.Select(ci => ci.ProductId).ToList();
        Products = client.GetProductsByIDs(productIds);

        Total = CartItems.Sum(item =>
        {
            var product = Products.FirstOrDefault(p => p.productId == item.ProductId);
            var price = product.onSale && product.SalePrice.HasValue ? product.SalePrice.Value : product.Price;
            return price * item.QtyAdded;
        });

        return Page();
    }

    public IActionResult OnPost()
    {
        int? userId = HttpContext.Session.GetInt32("USERID");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }

        // Fetch cart and items again
        UserCart = client.GetCart(userId.Value);
        CartItems = client.GetCartItems(UserCart.CartId);

        if (CartItems == null || !CartItems.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToPage("/Cart/Cart");
        }

        var productIds = CartItems.Select(ci => ci.ProductId).ToList();
        Products = client.GetProductsByIDs(productIds);

        var totalAmount = CartItems.Sum(item =>
        {
            var product = Products.FirstOrDefault(p => p.productId == item.ProductId);
            var price = product.onSale && product.SalePrice.HasValue ? product.SalePrice.Value : product.Price;
            return price * item.QtyAdded;
        });

        // Create Order
        var order = new Order
        {
            Date = DateTime.Now,
            Status = "Completed",
            OrderTotal = totalAmount,
            userId = userId.Value
        };

        int orderId = client.PlaceOrder(order, CartItems.Select(ci => new PurchaseProduct
        {
            productId = ci.ProductId,
            Qty = ci.QtyAdded
        }).ToList());

        if (orderId == -1)
        {
            TempData["ErrorMessage"] = "Failed to place the order.";
            return RedirectToPage("/Cart/Cart");
        }

        // Generate Invoice
        var invoice = client.GenerateInvoice(orderId);

        if (invoice == null)
        {
            TempData["ErrorMessage"] = "Failed to generate invoice.";
            return RedirectToPage("/Cart/Cart");
        }

        // Clear Cart
        foreach (var item in CartItems)
        {
            client.RemoveFromCartAsync(UserCart.CartId, item.ProductId).Wait();
        }

        // Redirect to Invoice Page
        return RedirectToPage("/Account/Invoices/Invoice", new { id = invoice.invoiceId });
    }
}