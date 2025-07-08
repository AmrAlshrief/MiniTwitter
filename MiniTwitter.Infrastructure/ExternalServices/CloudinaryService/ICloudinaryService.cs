using System;
using Microsoft.AspNetCore.Http;

namespace MiniTwitter.Infrastructure.ExternalServices.CloudinaryService;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile imagePath);
}
