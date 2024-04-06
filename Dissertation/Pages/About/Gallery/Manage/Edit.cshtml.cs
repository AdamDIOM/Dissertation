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
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.About.Gallery.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly UserManager<SiteUser> _userManager;

        public EditModel(Dissertation.Data.DissertationContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Image Image { get; set; } = default!;
        [BindProperty]
        public bool HomepageDisplay { get; set; } = default!;
        [BindProperty]
        public bool AdminUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image =  await _context.Image.FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }
            Image = image;

            HomepageDisplay = Image.HomepageBannerDisplay ?? true;

            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };

            AdminUser = (u != null && (await _userManager.IsInRoleAsync(u, "Admin")));

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Image.HomepageBannerDisplay = HomepageDisplay;

            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };

            AdminUser = (u != null && (await _userManager.IsInRoleAsync(u, "Admin")));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Image).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(Image.Id))
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

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
