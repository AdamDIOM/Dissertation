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
using Azure.Storage.Blobs;

namespace Dissertation.Pages.About.Team.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IConfiguration _config;

        public DeleteModel(Dissertation.Data.DissertationContext context, UserManager<SiteUser> um, IConfiguration config)
        {
            _context = context;
            _userManager = um;
            _config = config;
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
                if(Volunteer.Email != null)
                {
                    var user = await _userManager.FindByEmailAsync(Volunteer.Email);
                    if (user != null)
                    {
                        // take permissions
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                    }
                }

                string connSA = _config["SECRET_SA"] ?? "";
                if (connSA == "")
                {
                    connSA = _config.GetConnectionString("StorageAccount")!;
                }
                var blobServiceClient = new BlobServiceClient(connSA);

                //BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync("sponsor-images");
                BlobContainerClient container;

                container = blobServiceClient.GetBlobContainerClient("volunteer-images");
                if (!container.Exists())
                {
                    container = await blobServiceClient.CreateBlobContainerAsync("volunteer-images");
                }

                BlobClient blob = container.GetBlobClient($"{Volunteer.Id}-{Volunteer.Name}.png");
                if(blob != null && await blob.ExistsAsync())
                {
                    await blob.DeleteAsync();
                }

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
