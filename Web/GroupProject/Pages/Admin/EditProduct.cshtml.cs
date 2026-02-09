using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;
namespace GroupProject.Pages.Admin
{
    public class EditProductModel : PageModel
    {
        public WebServiceClient client { get; set; }

        [BindProperty]
        public ServiceReference1.Product product { get; set; } = new ServiceReference1.Product();
        public string success_msg { get; set; } = "";
        public string error_msg { get; set; } = "";

        public EditProductModel(WebServiceClient client)
        {
            this.client = client;
        }

        public void OnGet()
        {
            int prodId = Convert.ToInt32(Request.Query["ID"].ToString());

            var productInDb = client.GetProductByID(prodId);

            if(productInDb == null)
            {
                Response.Redirect("/Admin/ManageProducts");
                return;
            }

            product = productInDb;

        }

        public void OnPost()
        {
           
            //check if fields are provided
            if (product.ImageUrl == null)
            {
                ModelState.AddModelError("product.ImageUrl", "Image Url Required");
            }

            if (product.Name == null)
            {
                ModelState.AddModelError("product.Name", "Product Name Is Required");
            }

            if (product.Brand == null)
            {
                ModelState.AddModelError("product.Brand", "Product Brand Is Required");
            }

            if (product.Category == null)
            {
                ModelState.AddModelError("product.Category", "Product Category Is Required");
            }

            if (product.Price.ToString() == null || product.Price <= 0)
            {
                ModelState.AddModelError("product.Price", "Product Price Is Required");
            }


            if (!ModelState.IsValid)
            {
                error_msg = "Fill in required Fileds";
                return;
            }



                bool success = client.EditProductAsync(product).Result;
       
                if (!success)
                {
                    error_msg = "Failed to Update Product";
                    return;     
                }
   
                success_msg = "Product Successfully Updated";
                Response.Redirect("/Admin/ManageProducts");
        

        }
    }
}
