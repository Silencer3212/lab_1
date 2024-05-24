using System.ComponentModel.DataAnnotations;

namespace ProtectedWeb.Models
{
    public class Review
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Sneaker Sneaker { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }
}
