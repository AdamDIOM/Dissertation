using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.About.Team.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DeleteModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Volunteer Volunteer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteer.FirstOrDefaultAsync(m => m.Id == id);

            if (volunteer == null)
            {
                return NotFound();
            }
            else
            {
                Volunteer = volunteer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteer.FindAsync(id);
            if (volunteer != null)
            {
                Volunteer = volunteer;
                var pos = Volunteer.PagePosition;
                _context.Volunteer.Remove(Volunteer);
                List<Volunteer> volunteers = await _context.Volunteer.ToListAsync();
                foreach (var vol in volunteers)
                {
                    if(vol.PagePosition > pos)
                    {
                        vol.PagePosition--;
                        _context.Attach(vol).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
