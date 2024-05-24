namespace ProtectedWeb.Models
{
    public class Sneaker
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Logo { get; set; }
        public string? Description { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public Sneaker()
        {
            Reviews = new List<Review>();
        }
    }
}
