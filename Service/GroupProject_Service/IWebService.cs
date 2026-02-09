using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWebService" in both code and config file together.
    [ServiceContract]
    public interface IWebService
    {

        // User Management
        [OperationContract]
        bool Register(User user);

        [OperationContract]
        bool UserExists(string email);

        [OperationContract]
        User Login(string email, string password);

        [OperationContract]
        User GetUserById(int userId);

        [OperationContract]
        List<User> GetNewCustomers();

        // Product Management
        [OperationContract]
        List<Product> GetProducts();

        [OperationContract]
        List<Product> GetProductsOnSale();

        [OperationContract]
        List<Product> GetFeaturedProducts();

        [OperationContract]
        Product GetProductByID(int productId);

        [OperationContract]
        List<Product> GetProductsByCategory(string category);

        [OperationContract]
        List<string> GetCategories();

        [OperationContract]
        bool AddProduct(Product product);

        [OperationContract]
        bool EditProduct(Product product);

        [OperationContract]
        bool DeleteProduct(int productId);

        // Cart Management
        [OperationContract]
        bool AddToCart(int userId, int productId, int quantity);

        [OperationContract]
        Cart GetCart(int userId);

        [OperationContract]
        List<CartItem> GetCartItems(int cartId);

        [OperationContract]
        bool RemoveFromCart(int cartId, int productId);

        [OperationContract]
        decimal GetCartTotalPrice(int cartId);

        // Wishlist Management
        [OperationContract]
        bool AddToWishlist(int userId, int productId);

        [OperationContract]
        List<Product> GetWishlistItems(int userId);

        [OperationContract]
        bool RemoveFromWishlist(int userId, int productId);

        // Order Management
        [OperationContract]
        int PlaceOrder(Order order, List<PurchaseProduct> purchasedProducts);

        [OperationContract]
        Invoice GenerateInvoice(int orderId);

        [OperationContract]
        Invoice GetInvoiceByInvoiceID(int id);

        [OperationContract]
        List<Invoice> GetInvoicesByUserId(int userId);

        [OperationContract]
        List<Product> GetProductsOnSaleByCategory(string category);

        [OperationContract]
        Task<List<Product>> GetProductsByIDsAsync(List<int> productIds);

        [OperationContract]
        Order GetOrderById(int orderId);

        [OperationContract]
        List<PurchaseProduct> GetPurchasedProductsByOrderId(int orderId);

        [OperationContract]
        List<Product> GetPurchasedProducts();


        //report management
        [OperationContract]
        int GetNumberOfDifferentProductsSold();

        [OperationContract]
        int GetNumberOfRegisteredUsers(DateTime date);

        [OperationContract]
        int GetNumberOfOrders(DateTime date);

        [OperationContract]
        int GetTotalOrders();


        [OperationContract]
        decimal GetTotalRevenue();

        [OperationContract]
        decimal AverageRevenuePerUser();

        
    }
}
