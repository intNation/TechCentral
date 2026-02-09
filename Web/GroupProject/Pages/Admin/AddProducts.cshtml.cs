using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceReference1;

namespace GroupProject.Pages.Admin
{
    public class AddProductsModel : PageModel
    {
        public readonly WebServiceClient client;
        public string success_msg { get; set; } = "";
        public string error_msg { get; set; } = "";
        public Random random { get; } = new Random();
      
        public AddProductsModel(WebServiceClient client)
        {
            this.client = client;
        }


        [BindProperty]
        public ServiceReference1.Product product { get; set; } = new ServiceReference1.Product();
        public void OnGet()
        {
        }

        public void OnPost()
        {

            //check if fields are provided
            if(product.ImageUrl == null)
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

            //check  model state

            if (!ModelState.IsValid)
            {
                error_msg = "missing requred fields";
                return;
     
            }

            //save the product to server

            ServiceReference1.Product productToAdd = new ServiceReference1.Product
            {
                productId = random.Next(50,10000),
                Name = product.Name,
                Brand = product.Brand,
                Price = product.Price,
                SalePrice = product.SalePrice,
                Category = product.Category,
                Description = product.Description,
                isNew = true,
                ImageUrl = product.ImageUrl
                
            };

            bool success =  client.AddProductAsync(productToAdd).Result;

            if (success)
            {
                success_msg = "successfully added new product";
            }
            else{
                error_msg = "Failed To Add new Product, Either product id already exisist, or missing requred fields";
            }

            ModelState.Clear();
           

        }
    }
}
