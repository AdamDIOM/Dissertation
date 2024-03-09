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

        [BindProperty(SupportsGet = true)]
        public string? filter { get; set; }

        public async Task OnGetAsync()
        {
            Article = await _context.Articles.Where(a => a.PublishDate < DateTime.Now).ToListAsync();
            Tags = await _context.ArticleTags.ToListAsync();
            TagLinks = await _context.ArticleTagLinks.ToListAsync();
            if(filter != null && filter != "")
            {
                TagLinks = TagLinks.Where(tl => tl.TagId.ToString() == filter).ToList();
                Article = Article.Where(a => TagLinks.FirstOrDefault(tl => tl.ArticleId == a.Id) != null).ToList();
            }
            Article = Article.OrderByDescending(a => a.PublishDate).ToList();
        }

        public IActionResult OnPostFilter(string f)
        {
            filter = f;
            /*if (qry != null && qry != "")
            {
                return Redirect($"View/?qry={qry}&filter={filter}");
            }*/
            return Redirect($"News/?filter={filter}");
        }

        public IActionResult OnPostClearFilter()
        {
            filter = "";
            /*if (qry != null && qry != "")
            {
                return Redirect($"View/?qry={qry}");
            }*/
            return Redirect($"News");
        }
    }
}
