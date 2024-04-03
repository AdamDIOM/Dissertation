using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Pages.About.Team.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly IConfiguration _config;

        public CreateModel(Dissertation.Data.DissertationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            VolunteerTypes = await _context.VolunteerType.ToListAsync();
            return Page();
        }

        [BindProperty]
        public Volunteer Volunteer { get; set; } = default!;
        public IList<VolunteerType> VolunteerTypes { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Volunteer.PagePosition = await _context.Volunteer.CountAsync();
            _context.Volunteer.Add(Volunteer);
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

            return RedirectToPage("./Index");
        }
    }
}
