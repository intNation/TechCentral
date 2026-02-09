using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Reports
{
    public class ReportdataModel : PageModel
    {

        WebServiceClient client = new WebServiceClient();
        public List<ServiceReference1.Product> soldProds { get; set; }
        public int SoldUnits { get; set; }
        public List<User> newCusts { get; set; }
        public IActionResult OnGet(string data)
        {
            //check report data
            if(data == null)
            {
                return RedirectToPage("/Admin/Reports");
            }

            if (data.Equals("soldproducts"))
            {
                soldProds = client.GetPurchasedProducts();

                return Page();

            }else if (data.Equals("newcustomers"))
            {
                newCusts = client.GetNewCustomers();
                return Page();
            }

            return Page();
  
        }
    }
}
