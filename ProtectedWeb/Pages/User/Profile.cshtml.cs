using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProtectedWeb.Attributes.Permissions;
using ProtectedWeb.Utils;
using ProtectedWeb.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using ProtectedWeb.Attributes.Validations.User;
using Microsoft.AspNetCore.Routing.Constraints;

namespace ProtectedWeb.Pages.User
{
    [Authorized]
    [ValidateAntiForgeryToken]
    public class ProfileModel : PageModel
    {
        private readonly SneakersContext _context;
        private IWebHostEnvironment _appEnvironment;
        public string Email { get; private set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Не указано имя пользователя")]
        [StringLength(maximumLength: 15, MinimumLength = 5, ErrorMessage = "Имя пользователя должно содержать от 5 до 15 символов")]
        [BindProperty]
        public string Name { get; set; }
        public string Avatar { get; private set; }
        [BindProperty]
        public IFormFile? AvatarFile { get; set; }
        public string? Error { get; set; }

        public ProfileModel(SneakersContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public IActionResult OnGet(string? error)
        {
            Error = error;
            int userId = UserMethods.ValidateToken(HttpContext.Request.Cookies["access_token"]).Id;
            Models.User? user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            Email = user.Email;
            Name = user.Name;
            Avatar = user.Avatar;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                int userId = UserMethods.ValidateToken(HttpContext.Request.Cookies["access_token"]).Id;
                var userContext = _context.Users.FirstOrDefault(x => x.Id == userId);
                if (userContext == null)
                {
                    return NotFound();
                }
                if (AvatarFile != null)
                {
                    FileMethods fileLoader = new FileMethods
                    {
                        MaxSizeMb = 1,
                        AllowedImageExtentions = new() { ".jpg", ".jpeg", ".png", ".gif" },
                        Width = 320,
                        Height = 320,
                        Directory = "user",
                        AppEnvironment = _appEnvironment
                    };
                    string fileName = userContext.Id.ToString();
                    if (!fileLoader.UploadImage(AvatarFile, fileName))
                    {
                        return BadRequest();
                    }

                    userContext.Avatar = $"{fileName}.jpg";
                }           
                userContext.Name = Name;
                _context.SaveChanges();
                return RedirectToPage();
            }
            else
            {
                var errors = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage));
                return RedirectToPage("", new { error = errors });
            }
            return Page();
        }
    }
}
