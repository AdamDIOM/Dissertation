using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
string dbConnStr = "";
string msID = "";
string msSec = "";

try
{
    dbConnStr = config["SECRET_DB"] ?? "";
    if (dbConnStr == null || dbConnStr == "") throw new InvalidOperationException("no secrets file");
}
catch
{
    dbConnStr = builder.Configuration.GetConnectionString("DissertationContext")!;
}
try
{
    msID = config["AUTH_MS_ID"] ?? "";
    if (msID == null || msID == "") throw new InvalidOperationException("no secrets file");
}
catch
{
    msID = builder.Configuration.GetValue<string>("AUTH_MS_ID")!;
}
try
{
    msSec = config["AUTH_MS_SECRET"] ?? "";
    if (msSec == null || msSec == "") throw new InvalidOperationException("no secrets file");
}
catch
{
    msSec = builder.Configuration.GetValue<string>("AUTH_MS_SECRET")!;
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminAccess", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("VolunteerAccess", policy =>
    {
        policy.RequireRole("Volunteer", "Admin");
    });
    options.AddPolicy("MemberAccess", policy =>
    {
        policy.RequireRole("Member", "Volunteer", "Admin");
    });
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Volunteers", "VolunteerAccess");
    options.Conventions.AuthorizeFolder("/Members", "MemberAccess");
});

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.AuthorizationEndpoint = "https://login.microsoftonline.com/5eb26f0a-532d-45f6-b1b4-58c84e52a7c5/oauth2/v2.0/authorize";
    microsoftOptions.TokenEndpoint = "https://login.microsoftonline.com/5eb26f0a-532d-45f6-b1b4-58c84e52a7c5/oauth2/v2.0/token";
    microsoftOptions.ClientId = msID;
    microsoftOptions.ClientSecret = msSec;
});

builder.Services.AddDbContext<DissertationContext>(options =>
    options.UseSqlServer(dbConnStr ?? throw new InvalidOperationException("Connection string 'DissertationContext' not found.")));

builder.Services.AddIdentity<SiteUser, IdentityRole>()
    .AddEntityFrameworkStores<DissertationContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
});

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
