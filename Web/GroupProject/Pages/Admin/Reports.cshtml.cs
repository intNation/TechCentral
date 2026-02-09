using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Admin
{
    public class ReportsModel : PageModel
    {

        WebServiceClient client = new WebServiceClient();

        public int NumberOfProducts{ get; set; }
        public int RegisteredUsersToday { get; set; }
        public int NumberOfOrdersToday { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal avgRevenue { get; set; }

        public User user { get; set; }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            if(userId == null)
            {
                return RedirectToPage("/Account/Login/Login");
            }

            user = client.GetUserById((int)userId);

            //number of differennt product sold at the web site
            NumberOfProducts = client.GetNumberOfDifferentProductsSold();

            //number of registered users Today
            RegisteredUsersToday = client.GetNumberOfRegisteredUsers(DateTime.Today);

            //Total orders today
            NumberOfOrdersToday = client.GetNumberOfOrders(DateTime.Today);

            TotalOrders = client.GetTotalOrders();

            //total revenue
            TotalRevenue = client.GetTotalRevenue();

            //average revenue per user
            avgRevenue = client.AverageRevenuePerUser();

            return Page();
        }
    }
}
