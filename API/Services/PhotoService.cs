using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var cloudinaryAccount = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(cloudinaryAccount);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParam = new ImageUploadParams()
                {
                    File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                    Transformation = new CloudinaryDotNet.Transformation().Height(500)
                    .Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParam);
            }
            return uploadResult;
        }
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParam = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParam);
            return result;
        }

    }
}