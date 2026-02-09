using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Admin
{
    
    public class ManageProductsModel : PageModel
    {
       WebServiceClient client = new WebServiceClient();
        public List<ServiceReference1.Product> products { get; set; }

        public void OnGet()
        {
            products = client.GetProductsAsync().Result;
            if(products == null)
            {
                return;
            }


        }
    }
}
