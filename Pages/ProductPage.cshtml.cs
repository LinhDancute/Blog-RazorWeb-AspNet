using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppRazor.Pages
{
    public class ProductPageModel : PageModel
    {
        private readonly ILogger<ProductPageModel> _logger;
        public readonly ProductService _productService;

        public ProductPageModel(ILogger<ProductPageModel> logger, ProductService productService){
            _logger = logger;
            _productService = productService;
        }

        public Product product {set; get;}
        public void OnGet(int? id)
        {
            // var data = this.Request.Query["id"];
            // if (!string.IsNullOrEmpty(data)) 
            // {
            //     Console.WriteLine(data.ToString());
            //     int i = int.Parse(data.ToString());
            // }
            
            if (id != null)
            {
                ViewData["Title"] = $"San pham(ID = {id.Value})";
                product = _productService.Find(id.Value);
            } else {
                ViewData["Title"] = $"Danh sach san pham";
            }
        }

        public IActionResult OnGetLastProduct() {
            ViewData["Title"] = $"San pham cuoi";
            product = _productService.AllProducts().LastOrDefault();

            if (product != null)
            {
                return Page();
            } else
            {
                return this.NotFound();
            }
        }

        public IActionResult OnGetRemoveAll() {
            _productService.AllProducts().Clear();
            return RedirectToPage("ProductPage");
        }

        public IActionResult OnGetLoad()
        {
            _productService.LoadProducts();
            return RedirectToPage("ProductPage");
        }

        public IActionResult OnPostDelete(int? id) {
            if (id != null)
            {
                product = _productService.Find(id.Value);
                if (product != null)
                {
                    _productService.AllProducts().Remove(product);
                }
            }

            return this.RedirectToPage("ProductPage", new{id = string.Empty});
        }
    }
}
