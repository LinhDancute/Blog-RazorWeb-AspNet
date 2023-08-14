using Microsoft.AspNetCore.Mvc;

namespace AppRazor {
    public class ProductBox : ViewComponent {

        ProductService productService;
        public ProductBox(ProductService _products){
            productService = _products;
        }

        public IViewComponentResult Invoke(bool sapxeptang = true){

            List<Product> _product = null;
            if (sapxeptang)
            {
               // _product = productService.products.OrderBy(p => p.Price).ToList();
            } else {
               // _product = productService.products.OrderByDescending(p => p.Price).ToList();
            }

            return View<List<Product>>(_product);
        }
    }
}

