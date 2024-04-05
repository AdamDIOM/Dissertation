using Dissertation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Dissertation.Data.DissertationContext _context;
        public IndexModel(ILogger<IndexModel> logger, Dissertation.Data.DissertationContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IList<Article> Article { get; set; } = default!;
        public IList<Image> Image { get; set; } = default!;
        public Image ActiveImage { get; set; } = default!;
        public async Task OnGetAsync()
        {
            Article = await _context.Articles.Where(a => a.PublishDate < DateTime.Now && (a.HomepageDisplay ?? false)).ToListAsync();
            
            Article = Article.OrderByDescending(a => a.PublishDate).Take(3).ToList();

            Image = await _context.Image.Where(i => i.HomepageBannerDisplay ?? false).ToListAsync();
            ActiveImage = Image.First();
            Image.Remove(ActiveImage);
        }
    }
}
