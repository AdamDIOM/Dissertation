﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.News
{
    public class PostModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public PostModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public Article Article { get; set; } = default!;
        public IList<ArticleTag> Tags { get; set; } = default!;
        public IList<ArticleTagLink> Links { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FirstOrDefaultAsync(m => m.Slug == slug);
            if (article == null)
            {
                return NotFound();
            }
            else
            {
                Article = article;
            }
            Links = await _context.ArticleTagLinks.Where(l => l.ArticleId == Article.Id).ToListAsync();
            Tags = new List<ArticleTag>();
            var allTags = await _context.ArticleTags.ToListAsync();
            foreach (var link in Links)
            {
                var t = allTags.FirstOrDefault(t => t.Id == link.TagId);
                if (t != null) Tags.Add(t);
            }
            return Page();
        }
    }
}
