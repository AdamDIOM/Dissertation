using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Resources.Manage.Categories
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
        public ResourceType ResourceType { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ResourceType.Add(ResourceType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
