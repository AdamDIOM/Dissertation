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
        public MSLoginModel(
            SignInManager<SiteUser> signInManager, UserManager<SiteUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToPage("../Index");
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded) return RedirectToPage("../Index");
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
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return RedirectToPage("../Index");
                    }
                }
            }
            return RedirectToPage("Login");
        }
    }
}
