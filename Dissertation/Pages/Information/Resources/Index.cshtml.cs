using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Resources
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public IndexModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IList<Resource> Resource { get; set; } = default!;
        public IList<ResourceType> ResourceType { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Resource = await _context.Resource.OrderBy(r => r.PagePosition).ToListAsync();
            ResourceType = await _context.ResourceType.ToListAsync();
        }
    }
}
