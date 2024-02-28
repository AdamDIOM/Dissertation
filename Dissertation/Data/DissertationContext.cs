﻿using System;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>().ToTable("Article");
        }
    }
}
