﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.FAQs.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DeleteModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FAQ FAQ { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQ.FirstOrDefaultAsync(m => m.Id == id);

            if (faq == null)
            {
                return NotFound();
            }
            else
            {
                FAQ = faq;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQ.FindAsync(id);
            if (faq != null)
            {
                FAQ = faq;
                _context.FAQ.Remove(FAQ);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
