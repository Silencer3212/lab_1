using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProtectedWeb.Models;
using ProtectedWeb.Utils;
using System.ComponentModel.Design;

namespace ProtectedWeb.Pages.Sneaker
{
    public class CardInfo : Models.Sneaker
    {
        public int? Rating;
    }
    public class CatalogModel : PageModel
    {
        private readonly SneakersContext _context;
        public CatalogModel(SneakersContext context)
        {
            _context = context;
        }
        public int Page { get; private set; }
        public List<CardInfo> Sneakers { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize = 6;
        [BindProperty]
        public int? Id { get; set; }
        public bool isEmpty { get; set; }
        public IActionResult OnGet([FromQuery] int page)
        {
            Models.Sneaker[] allSneakers = _context.Sneakers.Include(u => u.Manufacturer).Include(x => x.Reviews).ToArray();
            if (allSneakers.Length == 0 || allSneakers == null)
            {
                isEmpty = true;
                return Page();
            }
            else
            {
                isEmpty = false;
            }
            TotalPages = (int)Math.Ceiling(allSneakers.Length / (double)PageSize);
            if (page > 0 && page <= TotalPages) {
                int skipAmount = (page - 1) * PageSize;
                Page = page;
                var sneakersData = allSneakers.OrderBy(p => p.Id)
                    .Skip(skipAmount)
                    .Take(PageSize)
                    .ToArray();
                Sneakers = new List<CardInfo> { };
                foreach (var sneakerData in sneakersData)
                {
                    int? rating;
                    var reviews = sneakerData.Reviews;
                    if (reviews == null || reviews.Count() == 0)
                    {
                        rating = null;
                    }
                    else
                    {
                        rating = (int)Math.Round((double)reviews.Average(x => x.Rating));
                    }
                    Sneakers.Add(new()
                    {
                        Id = sneakerData.Id,
                        Name = sneakerData.Name,
                        Logo = sneakerData.Logo,
                        Description = sneakerData.Description,
                        Manufacturer = sneakerData.Manufacturer,
                        Reviews = reviews,
                        Rating = rating
                    }
                    );
                }
                return Page();
            }
            else
            {
                return new NotFoundResult();
            }
        }
        public IActionResult OnPost()
        {
            string? token = HttpContext.Request.Cookies["access_token"];
            if (!UserMethods.HaveAdminRights(token))
            {
                return new ForbidResult();
            }
            if (Id == null)
            {
                return new BadRequestResult();
            }
            var sneaker = _context.Sneakers.FirstOrDefault(x => x.Id == Id);
            if (sneaker == null)
            {
                return new BadRequestResult();
            }
            _context.Sneakers.Remove(sneaker);
            _context.SaveChanges();
            return Redirect("/Sneaker/Catalog?page=1");
        }
    }
}
