using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;

namespace Dissertation.Pages.Information.Sponsors.Manage
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
            if (noImg == "true")
            {
                noImgMsg = "Image upload required.";
                ModelState.AddModelError("image", "No image uploaded.");
            }
            else
            {
                noImgMsg = "";
            }
            return Page();

        }

        [BindProperty]
        public Sponsor Sponsor { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? noImg { get; set; }
        public string noImgMsg { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Sponsor.ImageData = new byte[1];
            if (!ModelState.IsValid)
            {
                if (noImg == "true")
                {
                    noImgMsg = "Image upload required.";
                    ModelState.AddModelError("image", "No image uploaded.");
                }
                else
                {
                    noImgMsg = "";
                }
                return Page();

            }

            if (Request.Form.Files.Count < 1)
            {
                return Redirect("Create?noImg=true");
            }
            // copies file data into a memory stream and then into the object
            MemoryStream ms = new MemoryStream();
            Request.Form.Files[0].CopyTo(ms);
            Sponsor.ImageData = ms.ToArray();

            ms.Close();
            ms.Dispose();
            
            _context.Sponsor.Add(Sponsor);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
