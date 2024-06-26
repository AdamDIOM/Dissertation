﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Pages.Information.FAQs.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public CreateModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }
        public int LatestPosition;
        public async Task<IActionResult> OnGetAsync()
        {
            LatestPosition = (await _context.FAQ.ToListAsync()).Count;
            return Page();
        }

        [BindProperty]
        public FAQ FAQ { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            FAQ.PagePosition = await _context.FAQ.CountAsync();

            _context.FAQ.Add(FAQ);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
