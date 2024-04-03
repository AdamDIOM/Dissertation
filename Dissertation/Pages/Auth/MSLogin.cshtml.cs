using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Dissertation.Pages.Auth
{
    public class MSLoginModel : PageModel
    {
        private readonly SignInManager<SiteUser> _signInManager;
        private readonly UserManager<SiteUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public MSLoginModel(
            SignInManager<SiteUser> signInManager, UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> OnGetCallbackAsync(string ReturnUrl ="/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return Redirect(ReturnUrl);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded) return Redirect(ReturnUrl);
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                SiteUser user = new SiteUser { UserName = email, Email = email };
                var uresult = await _userManager.CreateAsync(user);
                if (uresult.Succeeded)
                {
                    uresult = await _userManager.AddLoginAsync(user, info);
                    if (uresult.Succeeded)
                    {
                        // creates volunteer role if not already existing
                        var r = await _roleManager.RoleExistsAsync("Volunteer");
                        if (!r)
                        {
                            var vRole = new IdentityRole { Name = "Volunteer" };
                            var roleResult = await _roleManager.CreateAsync(vRole);
                            if (roleResult.Succeeded)
                            {
                                if (user != null)
                                {
                                    await _userManager.AddToRoleAsync(user, "Volunteer");
                                }
                            }
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return Redirect(ReturnUrl);
                    }
                }
            }
            return Redirect(ReturnUrl);
        }
    }
}
