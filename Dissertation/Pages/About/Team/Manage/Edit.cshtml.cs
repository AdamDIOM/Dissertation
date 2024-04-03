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
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.About.Team.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SiteUser> _um;

        public EditModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> um)
        {
            _context = context;
            _config = config;
            _um = um;
        }

        [BindProperty]
        public Volunteer Volunteer { get; set; } = default!;
        public IList<VolunteerType> VolunteerTypes { get; set; } = default!;
        [BindProperty]
        public bool AdminPermissions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            VolunteerTypes = await _context.VolunteerType.ToListAsync();

            var volunteer =  await _context.Volunteer.FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null)
            {
                return NotFound();
            }
            Volunteer = volunteer;
            
            AdminPermissions = Volunteer.AdminPermissions ?? false;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string? move)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Volunteer.AdminPermissions = AdminPermissions;
            if(Volunteer.Email != null)
            {
                var user = await _um.FindByEmailAsync(Volunteer.Email);
                if (user != null)
                {
                    if (Volunteer.AdminPermissions ?? false)
                    {
                        // give permissions
                        await _um.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        // take permissions
                        await _um.RemoveFromRoleAsync(user, "Admin");
                    }
                }
                
            }

            if (move != null)
            {
                if (move == "up" && Volunteer.PagePosition > 0)
                {
                    var prevVol = await _context.Volunteer.FirstOrDefaultAsync(v => v.PagePosition == Volunteer.PagePosition - 1);
                    if (prevVol != null)
                    {
                        prevVol.PagePosition++;
                        Volunteer.PagePosition--;
                        _context.Attach(prevVol).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else if (move == "down" && Volunteer.PagePosition < (await _context.Volunteer.ToListAsync()).Count)
                {
                    var prevVol = await _context.Volunteer.FirstOrDefaultAsync(v => v.PagePosition == Volunteer.PagePosition + 1);
                    if (prevVol != null)
                    {
                        prevVol.PagePosition--;
                        Volunteer.PagePosition++;
                        _context.Attach(prevVol).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                }
                else
                {
                }
            }
            else
            {
            }

            _context.ChangeTracker.Clear();


            _context.Attach(Volunteer).State = EntityState.Modified;

            if (Request.Form.Files.Count >= 1)
            {
                // copies file data into a memory stream and then into the object
                MemoryStream ms = new MemoryStream();
                Request.Form.Files[0].CopyTo(ms);

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

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } });

                Volunteer.ImgUrl = $"https://dissertationad.blob.core.windows.net/volunteer-images/{Volunteer.Id}-{Volunteer.Name}.png";
                await _context.SaveChangesAsync();

                ms.Close();
                ms.Dispose();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(Volunteer.Id))
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

        private bool VolunteerExists(int id)
        {
            return _context.Volunteer.Any(e => e.Id == id);
        }
    }
}
