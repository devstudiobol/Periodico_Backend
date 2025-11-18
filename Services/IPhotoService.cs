using Microsoft.AspNetCore.Http;

namespace PeriodicoUpdate.Services
{
    public interface IPhotoService
    {
        // Define las "reglas":
        // 1. Un método para subir una foto
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        // 2. Un método para borrar una foto
        Task<bool> DeletePhotoAsync(string publicId);
    }
}