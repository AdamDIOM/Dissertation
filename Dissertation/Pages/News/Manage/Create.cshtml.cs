using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dissertation.Data;
using Dissertation.Models;
using System.Security.Cryptography.Xml;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Pages.News.Manage
{
    public class CreateModel : PageModel
    {
        private readonly Dissertation.Data.DissertationContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IConfiguration _config;

        public CreateModel(Dissertation.Data.DissertationContext context, IConfiguration config, UserManager<SiteUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        //public IActionResult OnGet()
        {
            Tags = await _context.ArticleTags.ToListAsync();

            var u = await _userManager.GetUserAsync(User) ?? new SiteUser { Email = "" };
            var email = await _userManager.GetEmailAsync(u);
            Volunteer? user = null;
            if (email != null) user = _context.Volunteer.FirstOrDefault(v => v.Email == email);
            Author = email ?? "";
            if (user != null && user.Name != null) Author = user.Name;
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; } = default!;
        [BindProperty]
        public IList<ArticleTag> Tags { get; set; } = default!;
        [BindProperty]
        public bool HomepageDisplay { get; set; } = default!;
        [BindProperty]
        public string Author { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Article.HomepageDisplay = HomepageDisplay;
            
            //Article.PublishDate = Article.PublishDate - TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            Tags = await _context.ArticleTags.ToListAsync();

                if(_context.Articles.Any(a => a.Slug == Article.Slug) && Article.Slug != null && Article.Slug != "")
                {
                    ModelState.AddModelError("Article.Slug", "The Slug must be unique for every article.");
                }

            if (Int32.TryParse(Article.Slug, out int _))
            {
                ModelState.AddModelError("Article.Slug", "The Slug must not be a number.");
            }

            if (Article.Slug != null && Article.Slug != "")
            {
                Article.Slug = Article.Slug.Replace(' ', '-');
            }

                if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Articles.Add(Article);

            await _context.SaveChangesAsync();

            if (Article.Slug == null || Article.Slug == "")
            {
                Article.Slug = Article.Id.ToString();
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

                //BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync("sponsor-images");
                BlobContainerClient container;

                container = blobServiceClient.GetBlobContainerClient("news-banner-images");
                if(!container.Exists())
                {
                    container = await blobServiceClient.CreateBlobContainerAsync("news-banner-images");
                }

                BlobClient blob = container.GetBlobClient(Article.Id.ToString() + ".png");

                BinaryData bd = new BinaryData(ms.ToArray());
                await blob.UploadAsync(content: bd, options: new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } });

                ms.Close();
                ms.Dispose();
            }

            foreach (ArticleTag t in Tags)
            {
                if (Request.Form[$"tagbox-{t.Id}"] == "true")
                {
                    _context.ArticleTagLinks.Add(new ArticleTagLink
                    {
                        ArticleId = Article.Id,
                        TagId = t.Id
                    });
                }
            }


            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
