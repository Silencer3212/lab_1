using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProtectedWeb.Attributes.Permissions;
using ProtectedWeb.Utils;
using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Pages.Sneaker
{
    [Admin]
    [ValidateAntiForgeryToken]
    public class EditModel : PageModel
    {
        private readonly SneakersContext _context;
        private IWebHostEnvironment _appEnvironment;
        public EditModel(SneakersContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [BindProperty]
        public int? Id { get; set; }
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
        public Models.Sneaker? Sneaker { get; set; }
        public IActionResult OnGet([FromQuery] int id)
        {
            Sneaker = _context.Sneakers.Include(u => u.Manufacturer).FirstOrDefault(x => x.Id == id);
            if (Sneaker == null)
            {
                return new BadRequestResult();
            }
            Manufacturers = _context.Manufacturers.ToList();
            return Page();

        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            Manufacturer = _context.Manufacturers.FirstOrDefault(x => x.Id == SelectedManufacturerId);

            var sneakerContext = _context.Sneakers.FirstOrDefault(x => x.Id == Id);
            if (sneakerContext == null)
            {
                return new BadRequestResult();
            }
            sneakerContext.Name = Name;
            sneakerContext.Description = Description;
            sneakerContext.Manufacturer = Manufacturer;

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
