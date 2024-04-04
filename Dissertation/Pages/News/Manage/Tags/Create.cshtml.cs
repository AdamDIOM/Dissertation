using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.News.Manage.Tags
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public CreateModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ArticleTag ArticleTag { get; set; } = default!;
        [BindProperty]
        public bool NavDisplay { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ArticleTag.NavDisplay = NavDisplay;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ArticleTags.Add(ArticleTag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
