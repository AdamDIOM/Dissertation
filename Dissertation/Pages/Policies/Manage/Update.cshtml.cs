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
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace Dissertation.Pages.Policies.Manage
{
    public class UpdateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SiteUser> _userManager;

        public UpdateModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty]
        public Policy Policy { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy =  await _context.Policy.FirstOrDefaultAsync(m => m.Id == id);
            if (policy == null)
            {
                return NotFound();
            }
            Policy = policy;


            Policy.DateUpdated = Policy.DateUpdated + TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            string updatedBy;

            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            updatedBy = email ?? "Volunteer";
            if (user != null && user.Name != null) updatedBy = user.Name;

            Policy.Link = $"https://dissertationad.blob.core.windows.net/policies/{Policy.Id}.pdf";
            if (Request.Form.Files.Count >= 1)
            {
                Policy.DateUpdated = DateTime.UtcNow;
                Policy.UploadedBy = updatedBy;
            }
                
            ModelState.Remove("Policy.DateUpdated");

            if (!ModelState.IsValid)
            {
                return Page();
            }


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

                BlobContainerClient container;

                container = blobServiceClient.GetBlobContainerClient("policies");
                if (!container.Exists())
                {
                    container = await blobServiceClient.CreateBlobContainerAsync("gallery");
                }

                BlobClient blob = container.GetBlobClient(Policy.Id.ToString() + ".pdf");

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "application/pdf" } });


                ms.Close();
                ms.Dispose();
            }

            _context.Attach(Policy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PolicyExists(Policy.Id))
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

        private bool PolicyExists(string id)
        {
            return _context.Policy.Any(e => e.Id == id);
        }
    }
}
