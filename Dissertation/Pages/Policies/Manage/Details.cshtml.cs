using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Policies.Manage
{
    public class DetailsModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DetailsModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public Policy Policy { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policy.FirstOrDefaultAsync(m => m.Id == id);
            if (policy == null)
            {
                return NotFound();
            }
            else
            {
                Policy = policy;
            }
            return Page();
        }
    }
}
