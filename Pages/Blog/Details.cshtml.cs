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
    [Authorize(Policy ="InGenZ")]
    public class DetailsModel : PageModel
    {
        private readonly AppRazor.models.AppDBContext _context;

        public DetailsModel(AppRazor.models.AppDBContext context)
        {
            _context = context;
        }

      public Article Article { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return NotFound();
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.ID == id);
            if (article == null)
            {
                return NotFound();
            }
            else 
            {
                Article = article;
            }
            return Page();
        }
    }
}
