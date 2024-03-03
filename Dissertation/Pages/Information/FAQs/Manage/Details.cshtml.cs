using System;
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
    public class DetailsModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DetailsModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

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
    }
}
