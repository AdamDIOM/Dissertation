using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.News.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public EditModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;
        public IList<ArticleTag> Tags { get; set; } = default!;
        public IList<ArticleTag> ChosenTags { get; set; } = default!;
        public IList<ArticleTagLink> Links { get; set; } = default!;
        [BindProperty]
        public bool HomepageDisplay { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article =  await _context.Articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            Article = article;
            HomepageDisplay = Article.HomepageDisplay ?? true;

            Links = await _context.ArticleTagLinks.Where(l => l.ArticleId == Article.Id).ToListAsync();
            Tags = await _context.ArticleTags.ToListAsync();
            ChosenTags = new List<ArticleTag>();
            var allTags = await _context.ArticleTags.ToListAsync();
            foreach (var link in Links)
            {
                var t = allTags.FirstOrDefault(t => t.Id == link.TagId);
                if (t != null) ChosenTags.Add(t);
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Article.BannerImage = Article.BannerImage?.ToArray();
            Article.HomepageDisplay = HomepageDisplay;

            Links = await _context.ArticleTagLinks.Where(l => l.ArticleId == Article.Id).ToListAsync();
            Tags = await _context.ArticleTags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Request.Form.Files.Count >= 1)
            {
                // copies file data into a memory stream and then into the object
                MemoryStream ms = new MemoryStream();
                Request.Form.Files[0].CopyTo(ms);
                Article.BannerImage = ms.ToArray();

                ms.Close();
                ms.Dispose();
            }
            _context.Attach(Article).State = EntityState.Modified;


            foreach (ArticleTag t in Tags)
            {
                if (Request.Form[$"tagbox-{t.Id}"] == "true")
                {
                    if(Links.FirstOrDefault(l => l.TagId == t.Id) == null)
                    {
                        _context.ArticleTagLinks.Add(new ArticleTagLink
                        {
                            ArticleId = Article.Id,
                            TagId = t.Id
                        });
                    }
                }
                else if(Links.FirstOrDefault(l => l.TagId == t.Id) != null)
                {
                    _context.Remove(Links.FirstOrDefault(l => l.TagId == t.Id)!);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
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
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
