using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Dissertation.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public IndexModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; } = default!;
        public IList<ArticleTag> Tags { get; set; } = default!;
        public IList<ArticleTagLink> TagLinks { get; set; } = default!;
        public int CategorySize { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? filter { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? p { get; set; }

        public async Task<IActionResult> OnGetAsync(int p = 1)
        {
            Article = await _context.Articles.Where(a => a.PublishDate < DateTime.Now).ToListAsync();
            Tags = await _context.ArticleTags.ToListAsync();
            TagLinks = await _context.ArticleTagLinks.ToListAsync();
            if(filter != null && filter != "" && filter != "all")
            {
                var fId = Tags.FirstOrDefault(t => t.Tag == filter);
                if(fId != null)
                {
                    TagLinks = TagLinks.Where(tl => tl.TagId == fId.Id).ToList();
                    if(TagLinks.Count() > 0)
                    {
                        Article = Article.Where(a => TagLinks.FirstOrDefault(tl => tl.ArticleId == a.Id) != null).ToList();
                    }
                    else
                    {
                        Article = new List<Article>();
                    }
                }
                else
                {
                    Article = new List<Article>();
                }
            }
            Article = Article.OrderByDescending(a => a.PublishDate).ToList();
            CategorySize = Article.Count();
            if(Article.Count == 0)
            {
                return Page();
            }
            if(Article.Count > ((p - 1) * 12))
            {
                Article = Article.Skip((p - 1) * 12).Take(12).ToList();
                return Page();
            }
            else if (filter != null && filter != "")
            {
                return Redirect($"../{filter}");
            }
            else
            {
                return Redirect("/News");
            }
        }
    }
}
