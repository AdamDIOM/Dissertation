using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;
using System.Runtime.InteropServices;

namespace Dissertation.Pages.News.Manage.Tags
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DeleteModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ArticleTag ArticleTag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articletag = await _context.ArticleTags.FirstOrDefaultAsync(m => m.Id == id);

            if (articletag == null)
            {
                return NotFound();
            }
            else
            {
                ArticleTag = articletag;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articletag = await _context.ArticleTags.FindAsync(id);
            if (articletag != null)
            {
                ArticleTag = articletag;
                _context.ArticleTags.Remove(ArticleTag);

                var tagLinks = await _context.ArticleTagLinks.Where(tl => tl.TagId == ArticleTag.Id).ToListAsync();

                foreach(var tl in tagLinks)
                {
                    _context.ArticleTagLinks.Remove(tl);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
