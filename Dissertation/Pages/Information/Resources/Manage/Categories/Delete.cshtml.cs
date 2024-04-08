using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Resources.Manage.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DeleteModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ResourceType ResourceType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourcetype = await _context.ResourceType.FirstOrDefaultAsync(m => m.Id == id);

            if (resourcetype == null)
            {
                return NotFound();
            }
            else
            {
                ResourceType = resourcetype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceType = await _context.ResourceType.FindAsync(id);
            if (resourceType != null)
            {
                ResourceType = resourceType;
                _context.ResourceType.Remove(ResourceType);

                var resources = await _context.Resource.Where(r => r.Category == ResourceType.Id).ToListAsync();
                foreach (var resource in resources)
                {
                    resource.Category = null;
                    _context.Attach(resource).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
