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

namespace Dissertation.Pages.Information.Sponsors.Manage
{
    public class EditModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;

        public EditModel(Dissertation.Data.DissertationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public Sponsor Sponsor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor =  await _context.Sponsor.FirstOrDefaultAsync(m => m.Id == id);
            if (sponsor == null)
            {
                return NotFound();
            }
            Sponsor = sponsor;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
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
                if(connSA == "")
                {
                    connSA = _config.GetConnectionString("StorageAccount")!;
                }
                var blobServiceClient = new BlobServiceClient(connSA);

                //BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync("sponsor-images");
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient("sponsor-images") ?? await blobServiceClient.CreateBlobContainerAsync("sponsor-images");

                BlobClient blob = container.GetBlobClient(Sponsor.Id.ToString() + ".png");

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } });

                ms.Close();
                ms.Dispose();
            }
            _context.Attach(Sponsor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SponsorExists(Sponsor.Id))
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

        private bool SponsorExists(int id)
        {
            return _context.Sponsor.Any(e => e.Id == id);
        }
    }
}
