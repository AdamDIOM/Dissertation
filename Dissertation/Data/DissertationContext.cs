using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dissertation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dissertation.Data
{
    public class DissertationContext : IdentityDbContext<SiteUser>
    {
        public DissertationContext (DbContextOptions<DissertationContext> options)
            : base(options)
        {
        }

        public DbSet<Dissertation.Models.Article> Articles { get; set; } = default!;
        public DbSet<Dissertation.Models.ArticleTag> ArticleTags { get; set; } = default!;
        public DbSet<Dissertation.Models.ArticleTagLink> ArticleTagLinks { get; set; } = default!;
        public DbSet<Dissertation.Models.Volunteer> Volunteer { get; set; } = default!;
        public DbSet<Dissertation.Models.VolunteerType> VolunteerType { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>().ToTable("Article");
        }
        public DbSet<Dissertation.Models.FAQ> FAQ { get; set; } = default!;
        public DbSet<Dissertation.Models.Sponsor> Sponsor { get; set; } = default!;
        public DbSet<Dissertation.Models.Image> Image { get; set; } = default!;
        public DbSet<Dissertation.Models.Resource> Resource { get; set; } = default!;
        public DbSet<Dissertation.Models.ResourceType> ResourceType { get; set; } = default!;
    }
}
