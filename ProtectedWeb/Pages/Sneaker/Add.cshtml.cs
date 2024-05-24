using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProtectedWeb.Attributes.Permissions;
using ProtectedWeb.Models;
using ProtectedWeb.Utils;
using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Pages.Sneaker
{
    [Admin]
    [ValidateAntiForgeryToken]
    public class AddModel : PageModel
    {
        private readonly SneakersContext _context;
        private IWebHostEnvironment _appEnvironment;
        public AddModel(SneakersContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Не указано название")]
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }
        [BindProperty]
        public IFormFile? Logo { get; set; }
        [Required(ErrorMessage = "Не указан производитель")]
        [BindProperty]
        public int SelectedManufacturerId { get; set; }
        public Models.Manufacturer Manufacturer { get; set; }
        public List<Models.Manufacturer> Manufacturers { get; set; }
        public string? File { get; set; }

        public void OnGet()
        {
            Manufacturers = _context.Manufacturers.ToList();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult(); 
            }

            Manufacturer = _context.Manufacturers.FirstOrDefault(x => x.Id == SelectedManufacturerId);

            Models.Sneaker sneaker = new()
            {
                Name = Name,
                Description = Description,
                Manufacturer = Manufacturer,
                Reviews = new List<Models.Review>() { },
                Logo = File
            };

            var sneakerContext = _context.Sneakers.Add(sneaker).Entity;
            _context.SaveChanges();

            if (Logo != null)
            {
                FileMethods fileLoader = new FileMethods
                {
                    MaxSizeMb = 1,
                    AllowedImageExtentions = new() { ".jpg", ".jpeg", ".png", ".gif" },
                    Width = 320,
                    Height = 320,
                    Directory = "sneaker",
                    AppEnvironment = _appEnvironment
                };
                string fileName = sneakerContext.Id.ToString();
                if (!fileLoader.UploadImage(Logo, fileName))
                {
                    return BadRequest();
                }

                sneakerContext.Logo = $"{fileName}.jpg";
            }

            _context.SaveChanges();
            return Redirect("/Sneaker/Catalog?page=1");
        }
    }
}
