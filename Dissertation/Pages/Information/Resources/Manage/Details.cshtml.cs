using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Resources.Manage
{
    public class DetailsModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public DetailsModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public Resource Resource { get; set; } = default!;
        public IList<ResourceType> ResourceType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource.FirstOrDefaultAsync(m => m.Id == id);
            ResourceType = await _context.ResourceType.ToListAsync();
            if (resource == null)
            {
                return NotFound();
            }
            else
            {
                Resource = resource;
            }
            return Page();
        }
    }
}
