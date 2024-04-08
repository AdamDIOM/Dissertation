using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.About.Gallery.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SiteUser> _userManager;

        public CreateModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> userManager)
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

            AdminUser = (user != null && (await _userManager.IsInRoleAsync(u, "Admin")));

            return Page();
        }

        [BindProperty]
        public Image Image { get; set; } = default!;
        [BindProperty]
        public bool HomepageDisplay { get; set; } = default!;
        [BindProperty]
        public string UploadedBy { get; set; } = default!;
        [BindProperty]
        public bool AdminUser { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            UploadedBy = email ?? "Volunteer";
            if (user != null && user.Name != null) UploadedBy = user.Name;

            AdminUser = (user != null && (await _userManager.IsInRoleAsync(u, "Admin")));

            Image.HomepageBannerDisplay = HomepageDisplay;

            ModelState.Remove(UploadedBy);

            if (Request.Form.Files.Count < 1)
            {
                ModelState.AddModelError("Image upload required");
            }

                if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Image.Add(Image);
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

                container = blobServiceClient.GetBlobContainerClient("gallery");
                if (!container.Exists())
                {
                    container = await blobServiceClient.CreateBlobContainerAsync("gallery");
                }

                BlobClient blob = container.GetBlobClient(Image.Id.ToString() + ".png");

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } });

                ms.Close();
                ms.Dispose();
            }


            return RedirectToPage("Index");
        }
    }
}
