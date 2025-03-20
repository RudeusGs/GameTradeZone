using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GameTradeZone.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _httpContextAccessor = new HttpContextAccessor();
        }
        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)
                       ?? _httpContextAccessor.HttpContext?.User.FindFirst("unique_name");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;
            return userId;
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var userId = GetCurrentUserId();
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"users/image/{userId}", 
                PublicId = $"{Guid.NewGuid()}", 
                Transformation = new Transformation().Width(500).Height(500).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        public async Task<List<string>> UploadMutilImage(List<IFormFile> files)
        {
            var userId = GetCurrentUserId();
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"auction/users/images/{userId}",
                    PublicId = $"{Guid.NewGuid()}",
                    Transformation = new Transformation().Width(500).Height(500).Crop("limit")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                imageUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
            }

            return imageUrls;
        }

        public async Task<string> UploadImageAvatarAsync(IFormFile file)
        {
            var userId = GetCurrentUserId();
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"users/avatar/{userId}",
                PublicId = $"{Guid.NewGuid()}",
                Transformation = new Transformation().Width(500).Height(500).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }
        public async Task DeleteImageAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        private static string GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments;
            var publicId = segments[segments.Length - 1];
            return publicId.Split('.')[0]; 
        }
    }
}

