using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

public class InvoiceModel : PageModel
{
    private readonly WebServiceClient client;

    public InvoiceModel(WebServiceClient client)
    {
        this.client = client;
    }

    public Invoice Invoice { get; set; }

    public List<Invoice> Invoicelist { get; set; }
    public Order Order { get; set; }
    public User user { get; set; }
    public List<PurchaseProduct> PurchasedProducts { get; set; }
    public List<Product> Products { get; set; }
    public string Layout { get; set; }

    public IActionResult OnGet(int id)
    {
        int? userId = HttpContext.Session.GetInt32("USERID");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login/Login");
        }


        Invoice = client.GetInvoiceByInvoiceID(id);

        if (Invoice == null)
        {
            TempData["ErrorMessage"] = "Invoice not found.";
            return RedirectToPage("/Index");
        }

        Order = client.GetOrderById(Invoice.orderId);

        if (Order == null || Order.userId != userId.Value)
        {
            TempData["ErrorMessage"] = "You are not allowed to view this invoice.";
            return RedirectToPage("/Index");
        }

        user = client.GetUserById(userId.Value);

        PurchasedProducts = client.GetPurchasedProductsByOrderId(Order.orderId);

        var productIds = PurchasedProducts.Select(pp => pp.productId).ToList();
        Products = client.GetProductsByIDs(productIds);

        return Page();
    }
}