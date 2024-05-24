using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProtectedWeb.Pages.User
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Response.Cookies.Delete("access_token");
            return RedirectToPage("/Index");
        }
    }
}
