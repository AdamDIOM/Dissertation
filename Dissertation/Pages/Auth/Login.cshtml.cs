using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Dissertation.Pages.Auth
{
    public class LoginModel : PageModel
    {
        [Required]
        [BindProperty]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [BindProperty]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        private readonly SignInManager<SiteUser> _signInManager;
        private readonly UserManager<SiteUser> _userManager;

        public LoginModel(SignInManager<SiteUser> sim, UserManager<SiteUser> um)
        {
            _signInManager = sim;
            _userManager = um;
        }
        public async Task OnGetAsync()
        {
            var u = await _userManager.FindByEmailAsync("site-admin@codeclub.im");
            if (u == null)
            {
                SiteUser admin = new SiteUser { Email = "site-admin@codeclub.im", UserName = "site-admin@codeclub.im" };
                var result = await _userManager.CreateAsync(admin, "C0deClub!");
            }
        }
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (ModelState.IsValid && Email != null && Password != null)
            {
                var result = await _signInManager.PasswordSignInAsync(Email, Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect("/Index");
                }
                ModelState.AddModelError(string.Empty, "Invalid Logon Attempt");
            }
            return Page();
        }

        public IActionResult OnPostLoginMsft()
        {
            var redirectUrl = Url.Page("./MSLogin", pageHandler: "Callback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
            return new ChallengeResult("Microsoft", properties);
        }
    }
}
