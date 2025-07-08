using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
namespace MiniTwitter.Infrastructure.ExternalServices.CloudinaryService;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
    }

    public async Task<string> UploadImageAsync(IFormFile imagePath)
    {
        if (imagePath == null || imagePath.Length <= 0)
            throw new ArgumentException("Image file is required.");

        using var stream = imagePath.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(imagePath.FileName, stream),
            Transformation = new Transformation().Width(800).Height(800).Crop("limit"),
            Folder = "miniTwitter/profile_pictures"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString();
    }
}
