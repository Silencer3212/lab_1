using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProtectedWeb.Attributes.Permissions;
using ProtectedWeb.Utils;
using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Pages.Sneaker
{
    [ValidateAntiForgeryToken]
    [Authorized]
    public class InfoModel : PageModel
    {
        private readonly SneakersContext _context;
        public Models.Sneaker Sneaker { get; set; }
        public double? ProductRating { get; set; }

        public Models.User Author { get; set; }

        [Required(ErrorMessage = "ќценка не может быть пустой")]
        [Range(1, 5, ErrorMessage = "ќценка должна быть от 1 до 5")]
        [BindProperty]
        public int Rating { get; set; }
        [BindProperty]
        public string? Comment { get; set; }
        [BindProperty]
        public int? SneakerId { get; set; }
        [BindProperty]
        public int? CommentId { get; set; }

        public InfoModel(SneakersContext context)
        {
            _context = context;
        }
        public IActionResult OnGet([FromQuery] int id)
        {
            Models.Sneaker? SneakerData = _context.Sneakers.Include(u => u.Manufacturer).Include(x => x.Reviews).FirstOrDefault(x => x.Id == id);
            if (SneakerData != null && SneakerData.Reviews != null)
            {
                foreach (var review in SneakerData.Reviews)
                {
                    _context.Entry(review).Reference(r => r.User).Load();
                }
            }
            else
            {
                return new NotFoundResult();
            }
            Sneaker = SneakerData;
            if (!(Sneaker.Reviews == null || Sneaker.Reviews.Count() == 0))
            {
                ProductRating = Sneaker.Reviews.Average(x => x.Rating);
            }
            else
            {
                ProductRating = null;
            }
            return Page();
        }
        public IActionResult OnPostSaveComment()
        {
            if (SneakerId == null)
            {
                return new BadRequestResult();
            }
            Models.Sneaker? SneakerData = _context.Sneakers.Include(u => u.Manufacturer).FirstOrDefault(x => x.Id == SneakerId);
            if (SneakerData == null)
            {
                return new NotFoundResult();
            }
            Sneaker = SneakerData;
            string? token = HttpContext.Request.Cookies["access_token"];
            if (token == null)
            {
                return new UnauthorizedResult();
            }
            Author = _context.Users.FirstOrDefault(x => x.Id == UserMethods.ValidateToken(token).Id);
            
            if (Author == null)
            {
                return new UnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            Models.Review review = new()
            {
                User = Author,
                Sneaker = Sneaker,
                Comment = Comment,
                Rating = Rating
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToPage("", new { id = SneakerId});
        }
        public IActionResult OnPostDeleteComment()
        {
            string? token = HttpContext.Request.Cookies["access_token"];
            if (!UserMethods.HaveModerRights(token))
            {
                return new ForbidResult();
            }
            if (CommentId == null)
            {
                return new BadRequestResult();
            }
            var comment = _context.Reviews.FirstOrDefault(x => x.Id == CommentId);
            if (comment == null)
            {
                return new BadRequestResult();
            }
            _context.Reviews.Remove(comment);
            _context.SaveChanges();
            return RedirectToPage("", new { id = SneakerId });
        }
    }
}
