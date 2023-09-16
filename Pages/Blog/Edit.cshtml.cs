using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppRazor.models;
using Microsoft.AspNetCore.Authorization;

namespace AppRazor.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly AppRazor.models.MyBlogContext _context;
        private readonly IAuthorizationService _authorizationService;
        public EditModel(AppRazor.models.MyBlogContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return Content("Không thấy bài viết");
            }

            var article =  await _context.articles.FirstOrDefaultAsync(m => m.ID == id);
            if (article == null)
            {
                return Content("Không thấy bài viết");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                //Kiểm tra quyền cập nhật bài viết blog
                var canUpdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                if (canUpdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Không được quyền cập nhật");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
          return (_context.articles?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
