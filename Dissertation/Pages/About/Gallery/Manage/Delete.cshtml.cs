using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dissertation.Data;
using Dissertation.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.About.Gallery.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SiteUser> _userManager;

        public DeleteModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> um)
        {
            _context = context;
            _config = config;
            _userManager = um;
        }

        [BindProperty]
        public Image Image { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FirstOrDefaultAsync(m => m.Id == id);

            if (image == null)
            {
                return NotFound();
            }
            else
            {
                Image = image;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image != null)
            {


                var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
                if (!(u != null && (await _userManager.IsInRoleAsync(u, "Admin"))))
                {
                    ModelState.AddModelError("Image.Id", $"Only admins or {image.UploadedBy} can delete this image.");
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var imageGet = await _context.Image.FirstOrDefaultAsync(m => m.Id == id);

                    if (imageGet == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        Image = imageGet;
                    }
                    return Page();

                }

                Image = image;

                string connSA = _config["SECRET_SA"] ?? "";
                if (connSA == "")
                {
                    connSA = _config.GetConnectionString("StorageAccount")!;
                }
                var blobServiceClient = new BlobServiceClient(connSA);

                //BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync("sponsor-images");
                BlobContainerClient container;

                container = blobServiceClient.GetBlobContainerClient("gallery");
                if (!container.Exists())
                {
                    container = await blobServiceClient.CreateBlobContainerAsync("gallery");
                }

                BlobClient blob = container.GetBlobClient($"{image.Id}.png");
                if (blob != null && await blob.ExistsAsync())
                {
                    await blob.DeleteAsync();
                }

                _context.Image.Remove(Image);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
