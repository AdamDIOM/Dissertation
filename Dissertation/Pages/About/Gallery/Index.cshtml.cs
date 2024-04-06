using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Dissertation.Pages.About.Gallery
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;

        public IndexModel(Dissertation.Data.DissertationContext context)
        {
            _context = context;
        }

        public IList<Image> Images { get;set; } = default!;

        public Image? Image { get; set; }
        public int? Before { get; set; }
        public int? After { get; set; }

        public async Task<IActionResult> OnGetAsync(int? image)
        {
            Images = await _context.Image.ToListAsync();

            if(image == null || Images == null)
            {
                return Page();
            }

            Image = Images.First(i => i.Id == image!);

            Before = Images.TakeWhile(i => i.Id != image).DefaultIfEmpty(Images.Last()).Last().Id;
            After = Images.SkipWhile(i => i.Id != image).Skip(1).DefaultIfEmpty(Images.First()).First().Id;
            return Page();
        }

    }
}
