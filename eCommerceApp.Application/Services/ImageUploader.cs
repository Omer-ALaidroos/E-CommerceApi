using Microsoft.AspNetCore.Http;

public class ImageUploader
{
    private readonly string _imageDirectory = Path.Combine("wwwroot", "images", "Products");

    public async Task<string?> UploadImage(IFormFile image)
    {
        // Validate file extension
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(image.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return null; // Invalid file extension
        }

        // Ensure the directory exists
        if (!Directory.Exists(_imageDirectory))
        {
            Directory.CreateDirectory(_imageDirectory);
        }

        // Generate a unique file name
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_imageDirectory, uniqueFileName);

        // Save the file
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return the relative path of the saved image
            return Path.Combine("images", "Products", uniqueFileName).Replace("\\", "/");
        }
        catch
        {
            return null; // Return null if saving fails
        }
    }
}