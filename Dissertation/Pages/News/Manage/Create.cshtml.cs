using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using System.Security.Cryptography.Xml;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Pages.News.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public CreateModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        //public IActionResult OnGet()
        {
            Tags = await _context.ArticleTags.ToListAsync();
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; } = default!;
        [BindProperty]
        public IList<ArticleTag> Tags { get; set; } = default!;
        [BindProperty]
        public bool HomepageDisplay { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //Article.BannerImage = new byte[0];
            Article.HomepageDisplay = HomepageDisplay;
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
            
            _context.Articles.Add(Article);
            await _context.SaveChangesAsync();

            foreach (ArticleTag t in Tags)
            {
                if (Request.Form[$"tagbox-{t.Id}"] == "true")
                {
                    _context.ArticleTagLinks.Add(new ArticleTagLink
                    {
                        ArticleId = Article.Id,
                        TagId = t.Id
                    });
                }
            }


            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
