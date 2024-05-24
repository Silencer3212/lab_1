using SixLabors.ImageSharp.Formats.Jpeg;
using static System.Net.Mime.MediaTypeNames;

namespace ProtectedWeb.Utils
{
    public class FileMethods
    {
        private int _maxSize;
        public int MaxSizeMb { get; set; }
        public List<string> AllowedImageExtentions { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IWebHostEnvironment AppEnvironment { get; set; }
        public string Directory { get; set; }
        public bool UploadImage(IFormFile uploadedFile, string fileName)
        {
            if (uploadedFile == null)
            {
                return false;
            }
            string extension = Path.GetExtension(uploadedFile.FileName);
            if (!AllowedImageExtentions.Any(ext => ext == extension.ToLower()))
            {
                return false;
            }
            if (uploadedFile.Length > 8 * 1024 * 1024 * MaxSizeMb)
            { 
                return false;
            }
            var image = SixLabors.ImageSharp.Image.Load(uploadedFile.OpenReadStream());
            if (image.Width < Width || image.Height < Height)
            {
                return false;
            }

            var resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Min,
                Size = new Size(Width)
            };
            image.Mutate(action => action.Resize(resizeOptions));

            var widthDifference = image.Width - Width;
            var heightDifference = image.Height - Height;
            var x = widthDifference / 2;
            var y = heightDifference / 2;

            var rectangle = new Rectangle(x, y, Width, Height);
            image.Mutate(action => action.Crop(rectangle));

            string filePath = Path.Combine(AppEnvironment.WebRootPath, "images", Directory, $"{fileName}.jpg");

            image.Save(filePath, new JpegEncoder { Quality = 75 });

            return true;
        }
    }
}
