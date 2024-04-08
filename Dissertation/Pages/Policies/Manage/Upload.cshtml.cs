using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using Microsoft.AspNetCore.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace Dissertation.Pages.Policies.Manage
{
    public class UploadModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SiteUser> _userManager;

        public UploadModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            UploadedBy = email ?? "Volunteer";
            if (user != null && user.Name != null) UploadedBy = user.Name;

            return Page();
        }

        [BindProperty]
        public Policy Policy { get; set; } = default!;
        [BindProperty]
        public string UploadedBy { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            UploadedBy = email ?? "Volunteer";
            if (user != null && user.Name != null) UploadedBy = user.Name;

            Policy.Id = Policy.Title;
            Policy.DateUpdated = DateTime.UtcNow;
            Policy.Link = $"https://dissertationad.blob.core.windows.net/policies/{Policy.Id}.pdf";
            ModelState.Remove("Policy.DateUpdated");

            if (Request.Form.Files.Count < 1)
            {
                ModelState.AddModelError("missing-policy", "Please upload policy");
            }
            if (_context.Policy.Any(p => p.Id == Policy.Id))
            {
                ModelState.AddModelError("already-exists", "This policy already exists");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Policy.Add(Policy);
            await _context.SaveChangesAsync();

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
                    container = await blobServiceClient.CreateBlobContainerAsync("policies");
                }

                BlobClient blob = container.GetBlobClient(Policy.Id.ToString() + ".pdf");

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "application/pdf" } });

                Policy.Link = $"https://dissertationad.blob.core.windows.net/policies/{Policy.Id}.pdf";

                ms.Close();
                ms.Dispose();
            }
            

            return RedirectToPage("./Index");
        }
    }
}
