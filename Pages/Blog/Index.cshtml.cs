using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppRazor.models;
using Microsoft.AspNetCore.Authorization;


namespace AppRazor.Pages_Blog
{
    [Authorize]

    public class IndexModel : PageModel
    {
        private readonly AppRazor.models.MyBlogContext _context;

        public IndexModel(AppRazor.models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;
        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage {get; set;}
        public int countPages {get; set;}

        public async Task OnGetAsync(string SearchString)
        {
            if (_context.articles != null)
            {
                int totalArticle = await _context.articles.CountAsync();
                countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;

                if (currentPage > countPages)
                    currentPage = countPages;
                

                var query = (from a in _context.articles
                            orderby a.Created descending
                            select a)
                            .Skip((currentPage - 1) * 10)
                            .Take(ITEMS_PER_PAGE);

                if (!string.IsNullOrEmpty(SearchString))
                {
                    // Apply the search filter using Where and assign the result back to query.
                    query = (IOrderedQueryable<Article>)query.Where(a => a.Title.Contains(SearchString));
                }

                // Execute the query and get the results.
                Article = await query.ToListAsync();
            }
        }
    }
}
