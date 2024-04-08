using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Resources.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public EditModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Resource Resource { get; set; } = default!;
        public IList<ResourceType> ResourceType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource =  await _context.Resource.FirstOrDefaultAsync(m => m.Id == id);
            ResourceType = await _context.ResourceType.ToListAsync();
            if (resource == null)
            {
                return NotFound();
            }
            Resource = resource;

            if (Resource.Category == null) Resource.Category = -1;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string? move)
        {
            ResourceType = await _context.ResourceType.ToListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Resource.Category == -1) Resource.Category = null;

            if (move != null)
            {
                if (move == "up" && Resource.PagePosition > 0)
                {
                    var prevResource = await _context.Resource.FirstOrDefaultAsync(r => r.PagePosition == Resource.PagePosition - 1);
                    if (prevResource != null)
                    {
                        prevResource.PagePosition++;
                        Resource.PagePosition--;
                        _context.Attach(prevResource).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else if (move == "down" && Resource.PagePosition < (await _context.Resource.ToListAsync()).Count)
                {
                    var prevResource = await _context.Resource.FirstOrDefaultAsync(r => r.PagePosition == Resource.PagePosition + 1);
                    if (prevResource != null)
                    {
                        prevResource.PagePosition--;
                        Resource.PagePosition++;
                        _context.Attach(prevResource).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                }
            }

            _context.ChangeTracker.Clear();

            _context.Attach(Resource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(Resource.Id))
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

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.Id == id);
        }
    }
}
