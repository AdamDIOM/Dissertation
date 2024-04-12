using AspNetCore.ReCaptcha;
using Dissertation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace Dissertation.Pages.About
{
    public class ContactInformation
    {
        [Required]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }

    }

    [ValidateReCaptcha]
    public class ContactModel : PageModel
    {

        private readonly SignInManager<SiteUser> _signInManager;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IConfiguration _config;

        public ContactModel(SignInManager<SiteUser> sim, UserManager<SiteUser> um, IConfiguration config)
        {
            _signInManager = sim;
            _userManager = um;
            _config = config;
        }
        [BindProperty]
        public ContactInformation ContactRequest { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? success { get; set; } = default!;
        public string? Email { get; set; }
        public async Task OnGetAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if(user != null)
                {
                    Email = await _userManager.GetEmailAsync(user);
                }
            }

        }

        public IActionResult OnPost()
        {
           
            Email = ContactRequest.Email;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                // gets email password
                string smtpPassword = _config["SECRET_SMTP_PASSWORD"] ?? "";
                // links to email client
                SmtpClient sc = new SmtpClient
                {
                    Credentials = new NetworkCredential("website@codeclub.im", smtpPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Host = "smtp.office365.com",
                    Port = 587
                };


                MailMessage m = new MailMessage();
                // from address
                m.From = new MailAddress("website@codeclub.im", "Code Club Website");
                // sent to address
                m.To.Add(new MailAddress("hello@codeclub.im"));
                // gives user's email as reply-to
                m.ReplyToList.Add(new MailAddress(ContactRequest.Email));
                // adds text and subject line then sends
                m.Subject = $"Website Enquiry \"{ContactRequest.Subject}\"";
                m.Body = $@"From: {ContactRequest.Name} <{ContactRequest.Email}>
Subject: {ContactRequest.Subject}


Message:
{ContactRequest.Message}
                            
--
This email was sent from the contact form on the Code Club website.";
                sc.Send(m);
                // provided nothing failed, redirects to confirmation page to show user their message.
                //return Redirect("/Contact/Confirm?Message=" + Message);
                return RedirectToPage(new { success = "true" });
            }
            // if something goes wrong, the page is reloaded with an error message
            catch (Exception e)
            {
                ModelState.AddModelError("Customer Contact Field Error", "Invalid data in customer contact form" + e.Message);
                return Page();
            }
        }
    }
}
