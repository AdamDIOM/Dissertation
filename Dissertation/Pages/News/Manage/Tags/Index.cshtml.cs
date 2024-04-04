using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.News.Manage.Tags
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public IndexModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IList<ArticleTag> ArticleTag { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ArticleTag = await _context.ArticleTags.ToListAsync();
        }

        public async Task<IActionResult> OnPostShowAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.ArticleTags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            tag.NavDisplay = true;
            _context.Attach(tag).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(tag.Id))
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

        public async Task<IActionResult> OnPostHideAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.ArticleTags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            tag.NavDisplay = false;
            _context.Attach(tag).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(tag.Id))
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

        private bool TagExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
