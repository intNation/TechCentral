using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Account.Invoices
{
    public class PreviousInvoicesModel : PageModel
    {
        private readonly WebServiceClient client;

        public PreviousInvoicesModel(WebServiceClient client)
        {
            this.client = client;
        }

        public List<Invoice> Invoices { get; set; }
        public string Layout { get; set; }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Invoices = client.GetInvoicesByUserIdAsync(userId.Value).Result;

            return Page();
        }
    }
}
