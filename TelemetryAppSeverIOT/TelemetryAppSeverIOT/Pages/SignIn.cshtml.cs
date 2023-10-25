using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TelemetryAppSeverIOT.Pages
{
    public class SignInModel : PageModel
    {
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        public void OnGet()
        {
        }
    }
}
