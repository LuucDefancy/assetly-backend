namespace BookingSystem.Services
{
  
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string fileName);
        string GetImageUrl(string fileName);
    }


    public class LocalFileStorageService : IImageStorageService
    {
        private readonly string _basePath;
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            // Bilder außerhalb von wwwroot speichern für mehr Kontrolle
            _basePath = Path.Combine(environment.ContentRootPath, "uploads", "devices");

            // Stelle sicher, dass der Ordner existiert
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Keine gültige Datei");

            // Validierung: nur Bilder erlauben
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Nur Bilddateien sind erlaubt");

            // Eindeutigen Dateinamen generieren
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_basePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // Nur den Dateinamen zurückgeben, nicht den ganzen Pfad
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var filePath = Path.Combine(_basePath, fileName);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                return true;
            }

            return false;
        }

        public string GetImageUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            return $"/api/device/image/{fileName}";
        }
    }
}
