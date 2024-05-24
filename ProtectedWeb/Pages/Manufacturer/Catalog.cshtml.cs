using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProtectedWeb.Attributes.Permissions;
using ProtectedWeb.Models;
using System;

namespace ProtectedWeb.Pages.Manufacturer
{
    [Admin]
    [ValidateAntiForgeryToken]
    public class CatalogModel : PageModel
    {
        private readonly SneakersContext _context;

        public CatalogModel(SneakersContext context)
        {
            _context = context;
        }
        public Models.Manufacturer[] Manufacturers { get; set; }
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string Name { get; set; }
        public void OnGet()
        {
            Manufacturers = _context.Manufacturers.ToArray();
        }
        public IActionResult OnPost(string action)
        {
            Models.Manufacturer? manufacturer = _context.Manufacturers.FirstOrDefault(x => x.Id == Id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            if (action == "save")
            {
                manufacturer.Name = Name;
            }
            else if (action == "delete")
            {
                _context.Manufacturers.Remove(manufacturer);
            }
            _context.SaveChanges();
            return RedirectToPage();
        }
    }
}
