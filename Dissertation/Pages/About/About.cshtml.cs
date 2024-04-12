using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dissertation.Pages.About
{
    public class AboutModel : PageModel
    {
        public int Age { get; set; }

        public AboutModel() { 
            Age = DateTime.Now.Year - 2014;
        }
        public void OnGet()
        {
            Age = DateTime.Now.Year - 2014;
        }

        public string Ordinal(int year)
        {
            var ones = year % 10;
            var tens = Math.Floor(year / 10f) % 10;
            if (tens == 1) return $"{year}th";
            switch (ones)
            {
                case 1: return $"{year}st";
                case 2: return $"{year}nd";
                case 3: return $"{year}rd";
                default: return $"{year}th";
            }
        }
    }
}
