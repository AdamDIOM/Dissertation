using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DissertationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DissertationContext") ?? throw new InvalidOperationException("Connection string 'DissertationContext' not found.")));

builder.Services.AddIdentity<SiteUser, IdentityRole>()
    .AddEntityFrameworkStores<DissertationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
