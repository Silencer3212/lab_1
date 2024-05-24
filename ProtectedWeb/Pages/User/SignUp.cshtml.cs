using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProtectedWeb.Utils;
using ProtectedWeb.Models;
using ProtectedWeb.Attributes.Validations.User;
using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Pages.User
{
    [ValidateAntiForgeryToken]
    public class SignUpModel : PageModel
    {
        private readonly SneakersContext _context;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Не указано имя пользователя")]
        [StringLength(maximumLength: 15, MinimumLength = 5, ErrorMessage = "Имя пользователя должно содержать от 5 до 15 символов")]
        [BindProperty]
        public string Login { get; set; }
        [Required(ErrorMessage = "Не указан e-mail")]
        [EmailAddress(ErrorMessage = "Не верный e-mail")]
        [UniqueEmail(ErrorMessage = "Email должен быть уникальным")]
        [BindProperty]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Не указан пароль")]
        [StringLength(maximumLength: 15, MinimumLength = 5, ErrorMessage = "Пароль должен содержать от 5 до 15 символов")]
        [BindProperty]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [BindProperty]
        public string PasswordConfirm { get; set; }
        public string? Error { get; set; }

        public SignUpModel(SneakersContext context)
        {
            _context = context;
        }
        public void OnGet(string? error)
        {
            Error = error;
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                Models.User user = new Models.User
                {
                    Name = Login,
                    Email = Email,
                    HashedPassword = UserMethods.GetPasswordHash(Password),
                    Role = _context.Roles.ToList().Find(x => x.Id == 3),
                    Avatar = "placeholder.jpg"
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToPage("/User/SignUpSuccesful");
            }
            else
            {
                var errors = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage));
                return RedirectToPage("", new { error = errors});
            }
        }
    }
}
