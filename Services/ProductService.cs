using System.Linq;
namespace AppRazor {
    public class ProductService {
        private List<Product> products = new List<Product>{
            // new Product() {Name = "SP1", Description = "Apple", Price = 1600},
            // new Product() {Name = "SP2", Description = "Apple", Price = 1200},
            // new Product() {Name = "SP3", Description = "Samsung", Price = 900},
            // new Product() {Name = "SP4", Description = "Oppo", Price = 700},
        };

        public ProductService(){
            LoadProducts();
        }

        public Product Find(int id){
            var qr = from p in products 
                     where p.Id == id select p;
            return qr.FirstOrDefault();
        }

        public List<Product> AllProducts() => products;

        public void LoadProducts(){
            products.Clear();
            products.Add(new Product() {
                Id = 1,
                Name = "Iphone",
                Description = "Apple",
                Price = 2000
            });
            products.Add(new Product()
            {
                Id = 2,
                Name = "Samsung",
                Description = "Samsung",
                Price = 1000
            });
            products.Add(new Product()
            {
                Id = 3,
                Name = "Oppo",
                Description = "Oppo",
                Price = 1200
            });
        }
    }
}