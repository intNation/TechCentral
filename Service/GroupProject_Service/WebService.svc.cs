using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using HashPass;

namespace GroupProject_Service
{
   
    public class WebService : IWebService
    {
        LinqDataContext dc = new LinqDataContext();
        // User Management
        public bool Register(User user)
        {
            if (UserExists(user.Email))
            {
                return false;
            }

            user.Password = Secrecy.HashPassword(user.Password);
            dc.Users.InsertOnSubmit(user);

            try
            {
                dc.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UserExists(string email)
        {
            bool exist = (from u in dc.Users where u.Email == email select u).Any();
            return exist;
        }

        public User Login(string email, string password)
        {
            string hashedPassword = Secrecy.HashPassword(password);
            var user = (from u in dc.Users where u.Email == email && u.Password == hashedPassword select u).FirstOrDefault();

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }

           
        }

        public User GetUserById(int userId)
        {
            var user = (from u in dc.Users where u.userId == userId select u).FirstOrDefault();

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }

        }

        public List<User> GetNewCustomers()
        {
            try
            {
                DateTime today = DateTime.Today;

                var newCustomers = (from u in dc.Users
                    where u.RegistrationDate.Date == today select u)
                    .ToList();

                if(newCustomers != null)
                {

                    return newCustomers;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<User>();
            }
        }


        // Product Management
        public List<Product> GetProducts()
        {
            dynamic prods = (from p in dc.Products select p).DefaultIfEmpty();
            List<Product> toReturn = new List<Product>();
            if (prods != null)
            {
                foreach(Product p in prods)
                {
                    toReturn.Add(p);
                }
                return toReturn;
            }
            else
            {
                return null;
            }
        }

        public List<Product> GetProductsOnSale()
        {
            dynamic prods = (from p in dc.Products where p.onSale == true select p).ToList();
            List<Product> toReturn = new List<Product>();
            if (prods != null)
            {
                foreach(Product p in prods)
                {
                    toReturn.Add(p);
                }
                return toReturn;
            }
            else
            {
                return null;
            }
        }

        public List<Product> GetFeaturedProducts()
        {
           dynamic prods = (from p in dc.Products where p.isNew == true select p).DefaultIfEmpty();
            List<Product> toReturn = new List<Product>();
            if (prods != null)
            {
                foreach(Product p in prods)
                {
                    toReturn.Add(p);
                }
                return toReturn;
            }
            else
            {
                return null;
            }
        }

        public Product GetProductByID(int productId)
        {
            var prod = (from p in dc.Products where p.productId == productId select p).FirstOrDefault();
          
            if (prod != null)
            {        
                return prod;
            }
            else
            {
                return null;
            }
        }

        public List<Product> GetProductsByCategory(string category)
        {
            dynamic prods = (from p in dc.Products where p.Category == category select p).ToList();
            List<Product> toReturn = new List<Product>();
            if (prods != null)
            {
                foreach (Product p in prods)
                {
                    toReturn.Add(p);
                }
                return toReturn;
            }
            else
            {
                return null;
            }
        }

        public List<string> GetCategories()
        {
            return dc.Products.Select(p => p.Category).Distinct().ToList();
        }

        public bool AddProduct(Product product)
        {
            if (dc.Products.Any(p => p.Name == product.Name))
            {
                return false;
            }

            dc.Products.InsertOnSubmit(product);

            try
            {
                dc.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool EditProduct(Product editedProduct)
        {
           
            var productInDb = (from p in dc.Products where p.productId == editedProduct.productId select p).FirstOrDefault();



            if (productInDb != null)
            {
                productInDb.Name = editedProduct.Name;
                productInDb.Brand = editedProduct.Brand;
                productInDb.Category = editedProduct.Category;
                productInDb.Price = editedProduct.Price;
                productInDb.SalePrice = editedProduct.SalePrice;
                productInDb.Description = editedProduct.Description;
                productInDb.ImageUrl = editedProduct.ImageUrl;
                productInDb.onSale = editedProduct.onSale;
                productInDb.isNew = editedProduct.isNew;
                productInDb.StockQty = editedProduct.StockQty;

                try
                {
                    dc.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool DeleteProduct(int productId)
        {
            var productInDb = (from p in dc.Products where p.productId == productId select p).FirstOrDefault();

            if (productInDb != null)
            {
                dc.Products.DeleteOnSubmit(productInDb);

                try
                {
                    dc.SubmitChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return false;
        }

        // Cart Management
        public bool AddToCart(int userId, int productId, int quantity)
        {
            var cart = (from c in dc.Carts where c.UserId == userId select c).FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                dc.Carts.InsertOnSubmit(cart);
                dc.SubmitChanges();
            }


            var cartitem = (from ci in dc.CartItems where ci.CartId == cart.CartId && ci.ProductId == productId select ci).FirstOrDefault();

            if (cartitem != null)
            {
                cartitem.QtyAdded += quantity;
            }
            else
            {
                cartitem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    QtyAdded = quantity
                };
                dc.CartItems.InsertOnSubmit(cartitem);
            }

            try
            {
                dc.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Cart GetCart(int userId)
        {
            var cart = (from c in dc.Carts where c.UserId == userId select c).FirstOrDefault();

            if(cart != null)
            {
                return cart;
            }
            else
            {
                return null;
            }
        }

        public List<CartItem> GetCartItems(int cartId)
        {
           
            dynamic cartitems = (from ci in dc.CartItems where ci.CartId == cartId select ci).ToList();
            List<CartItem> toReturn = new List<CartItem>();
            if (cartitems != null)
            {
                foreach (CartItem ci in cartitems)
                {
                    toReturn.Add(ci);
                }
                return toReturn;
            }
            else
            {
                return null;
            }


        }


        public bool RemoveFromCart(int cartId, int productId)
        {
            

            var cartItem = (from ci in dc.CartItems where ci.CartId == cartId && ci.ProductId == productId select ci).FirstOrDefault();


            if (cartItem != null)
            {
                dc.CartItems.DeleteOnSubmit(cartItem);

                try
                {
                    dc.SubmitChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return false;
        }

        public decimal GetCartTotalPrice(int cartId)
        {
            var cartItems = GetCartItems(cartId);

            decimal totalPrice = 0;

            foreach (var item in cartItems)
            {
                var product = GetProductByID(item.ProductId);
                decimal price = product.onSale ? product.SalePrice ?? product.Price : product.Price;
                totalPrice += price * item.QtyAdded;
            }

            return totalPrice;
        }

        // Wishlist Management
        public bool AddToWishlist(int userId, int productId)
        {
            var wishlist = dc.WishLists.FirstOrDefault(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new WishList { UserId = userId };
                dc.WishLists.InsertOnSubmit(wishlist);
                dc.SubmitChanges(); 
            }

            var existingItem = dc.WishListItems.FirstOrDefault(wi => wi.WishListId == wishlist.WishListId && wi.ProductId == productId);

            if (existingItem != null)
            {
                // Item already in wishlist
                return false;
            }

            var wishListItem = new WishListItem
            {
                WishListId = wishlist.WishListId,
                ProductId = productId
            };

            dc.WishListItems.InsertOnSubmit(wishListItem);

            try
            {
                dc.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<Product> GetWishlistItems(int userId)
        {
            var wishlist = dc.WishLists.FirstOrDefault(w => w.UserId == userId);

            if (wishlist != null)
            {
                var productIds = dc.WishListItems.Where(wi => wi.WishListId == wishlist.WishListId).Select(wi => wi.ProductId).ToList();
                return dc.Products.Where(p => productIds.Contains(p.productId)).ToList();
            }

            return new List<Product>();
        }

        public bool RemoveFromWishlist(int userId, int productId)
        {
            var wishlist = dc.WishLists.FirstOrDefault(w => w.UserId == userId);

            if (wishlist != null)
            {
                var wishListItem = dc.WishListItems.FirstOrDefault(wi => wi.WishListId == wishlist.WishListId && wi.ProductId == productId);

                if (wishListItem != null)
                {
                    dc.WishListItems.DeleteOnSubmit(wishListItem);

                    try
                    {
                        dc.SubmitChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }

            return false;
        }

        // Order Management
        public int PlaceOrder(Order order, List<PurchaseProduct> purchasedProducts)
        {
            dc.Orders.InsertOnSubmit(order);

            try
            {
                dc.SubmitChanges();

                foreach (var purchasedProduct in purchasedProducts)
                {
                    purchasedProduct.orderId = order.orderId;
                    dc.PurchaseProducts.InsertOnSubmit(purchasedProduct);
                }

                dc.SubmitChanges();
                return order.orderId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public Invoice GenerateInvoice(int orderId)
        {
         
            var order = (from o in dc.Orders where o.orderId == orderId select o).FirstOrDefault();

            if (order != null)
            {
                var invoice = new Invoice
                {
                    Date = DateTime.Now,
                    Total = order.OrderTotal,
                    Status = true, 
                    orderId = orderId
                };

                dc.Invoices.InsertOnSubmit(invoice);

                try
                {
                    dc.SubmitChanges();
                    return invoice;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return null;
        }

        public List<Invoice> GetInvoicesByUserId(int userId)
        {
            var orders = dc.Orders.Where(o => o.userId == userId).Select(o => o.orderId).ToList();
            return dc.Invoices.Where(i => orders.Contains(i.orderId)).ToList();
        }

        public  Invoice GetInvoiceByInvoiceID(int id)
        {
            
            var invoice = (from i in dc.Invoices where i.invoiceId == id  select i).FirstOrDefault();

            if(invoice !=null)
            {
                return invoice;
            }
            else{
                return null;
            }

        }


        public List<Product> GetProductsOnSaleByCategory(string category)
        {

          
            dynamic prods = (from p in dc.Products where p.onSale && p.Category == category select p).ToList();
            List<Product> toReturn = new List<Product>();
            if (prods != null)
            {
                foreach (Product p in prods)
                {
                    toReturn.Add(p);
                }
                return toReturn;
            }
            else
            {
                return null;
            }

        }

       
        public async Task<List<Product>> GetProductsByIDsAsync(List<int> productIds)
        {
            return await Task.Run(() => dc.Products.Where(p => productIds.Contains(p.productId)).ToList());
        }

        public Order GetOrderById(int orderId)
        {
           
            var order = (from o in dc.Orders where o.orderId == orderId select o).FirstOrDefault();

            if (order != null)
            {
                return order;
            }
            else
            {
                return null;
            }

        }

        public List<PurchaseProduct> GetPurchasedProductsByOrderId(int orderId)
        {
           
            dynamic prods = (from pp in dc.PurchaseProducts where pp.orderId == orderId select pp).ToList();
            
            if (prods != null)
            {
                return prods;
            }
            else
            {
                return null;
            }

        }


        public List<Product> GetPurchasedProducts()
        {
            try
            {
                // Get a list of product IDs that have been purchased
                var purchasedProductIds = dc.PurchaseProducts.Select(pp => pp.productId).Distinct().ToList();

                var purchasedProducts = dc.Products
                    .Where(p => purchasedProductIds.Contains(p.productId))
                    .ToList();

                return purchasedProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Product>(); // Return an empty list or handle the exception accordingly
            }
        }

        public int GetNumberOfDifferentProductsSold()
        {
            return dc.PurchaseProducts.Select(pp => pp.productId).Distinct().Count();
        }

        public int GetNumberOfRegisteredUsers(DateTime date)
        {
            return dc.Users.Count(u => u.RegistrationDate == date.Date);
        }

        public int GetNumberOfOrders(DateTime date)
        {
            return dc.Orders.Count(o => o.Date.Date == date.Date);
        }


        public int GetTotalOrders()
        {
            return dc.Orders.Count();
        }

        public decimal GetTotalRevenue()
        {
            try
            {
                decimal totalRevenue = dc.Orders.Sum(o => o.OrderTotal);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0; 
            }
        }

        public decimal AverageRevenuePerUser()
        {
            try
            {
            
                decimal totalRevenue = dc.Orders.Sum(o => o.OrderTotal);

                int userCount = dc.Orders.Select(o => o.userId).Distinct().Count();

             
                return userCount > 0 ? totalRevenue / userCount : 0; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0; 
            }
        }
    }


}
