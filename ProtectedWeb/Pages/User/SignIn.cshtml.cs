using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ProtectedWeb.Utils;
using ProtectedWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ProtectedWeb.Pages.User
{
    public class SignInModel : PageModel
    {
        private readonly SneakersContext _context;

        public SignInModel(SneakersContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost(string email, string password)
        {
            var identity = GetIdentity(email, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            HttpContext.Response.Cookies.Append("access_token", encodedJwt);
            return RedirectToPage("/Index");
        }

        public ClaimsIdentity? GetIdentity(string email, string password)
        {
            Models.User? user = _context.Users
                .Include(u => u.Role)
                .ToList().FirstOrDefault(
                x => x.Email == email && x.HashedPassword == UserMethods.GetPasswordHash(password)
                );
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role.Id.ToString())
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }
            return null;
        }
    }
}
