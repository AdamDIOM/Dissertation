using Microsoft.AspNetCore.Identity;

namespace Dissertation.Data
{
    public class SiteUser: IdentityUser
    {
        public string? FirstName { get; set; }
    }
}
