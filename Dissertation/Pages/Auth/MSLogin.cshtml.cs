using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace Dissertation.Pages.Auth
{
    public class MSLoginModel : PageModel
    {
        private readonly SignInManager<SiteUser> _signInManager;
        private readonly UserManager<SiteUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Dissertation.Data.DissertationContext _context;
        public MSLoginModel(
            SignInManager<SiteUser> signInManager, UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager, DissertationContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> OnGetCallbackAsync(string ReturnUrl ="/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return Redirect(ReturnUrl);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                SiteUser? user = await _userManager.GetUserAsync(User);
                if(user != null)
                {
                    var hasRole = await _userManager.IsInRoleAsync(user, "Volunteer");
                }
                return Redirect(ReturnUrl);
            }
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
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "Volunteer");
                        }
                        if(_context.Volunteer.Any(v => (v.Email?? "noemail1").ToLower() == (email ?? "noemail2").ToLower() && v.AdminPermissions != null && (bool)v.AdminPermissions))
                        {
                            if(user != null)
                            {
                                await _userManager.AddToRoleAsync(user, "Admin");
                            }
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return Redirect(ReturnUrl);
                    }
                }
            }
            return Redirect(ReturnUrl);
        }
    }
}
