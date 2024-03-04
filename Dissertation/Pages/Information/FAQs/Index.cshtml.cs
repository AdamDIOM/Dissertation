using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.FAQs
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public IndexModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IList<FAQ> FAQ { get;set; } = default!;

        public async Task OnGetAsync()
        {
            FAQ = (await _context.FAQ.ToListAsync()).OrderBy(f => f.PagePosition).ToList();
        }
    }
}
