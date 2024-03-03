﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.FAQs.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public EditModel(Dissertation.Data.DissertationContext context)
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

            var faq =  await _context.FAQ.FirstOrDefaultAsync(m => m.Id == id);
            if (faq == null)
            {
                return NotFound();
            }
            FAQ = faq;
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

            _context.Attach(FAQ).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FAQExists(FAQ.Id))
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

        private bool FAQExists(int id)
        {
            return _context.FAQ.Any(e => e.Id == id);
        }
    }
}
