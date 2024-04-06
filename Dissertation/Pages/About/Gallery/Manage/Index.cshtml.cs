using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.About.Gallery.Manage
{
    public class IndexModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly UserManager<SiteUser> _userManager;

        public IndexModel(Dissertation.Data.DissertationContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [BindProperty]
        public string CurrentUser { get; set; } = default!;
        [BindProperty]
        public bool AdminUser { get; set; } = default!;

        public IList<Image> Image { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            CurrentUser = email ?? "Volunteer";
            if (user != null && user.Name != null) CurrentUser = user.Name;

            AdminUser = (user != null && (await _userManager.IsInRoleAsync(u, "Admin")));
            Image = await _context.Image.ToListAsync();
        }
    }
}
