using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Admin
{
    public class DeleteProductModel : PageModel
    {
        WebServiceClient client = new WebServiceClient();
        public DeleteProductModel(WebServiceClient client)
        {
            this.client = client;
        }

        public void OnGet()
        {
            int prodId = Convert.ToInt32(Request.Query["ID"].ToString());

            var productInDb = client.GetProductByID(prodId);

            if (productInDb == null)
            {
                Response.Redirect("/Admin/ManageProducts");
                return;
            }

            client.DeleteProductAsync(productInDb.productId);
            Response.Redirect("/Admin/ManageProducts");

        }
    }
}
