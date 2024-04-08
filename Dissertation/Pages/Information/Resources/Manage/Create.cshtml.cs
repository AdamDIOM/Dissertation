using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Pages.Information.Resources.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public CreateModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ResourceType = await _context.ResourceType.ToListAsync();
            return Page();
        }

        [BindProperty]
        public Resource Resource { get; set; } = default!;
        public IList<ResourceType> ResourceType { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ResourceType = await _context.ResourceType.ToListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Resource.Category == -1) Resource.Category = null;

            Resource.PagePosition = await _context.Resource.CountAsync();

            _context.Resource.Add(Resource);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
