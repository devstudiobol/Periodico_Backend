using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PeriodicoUpdate.Models; // Necesario para CloudinarySettings
using PeriodicoUpdate.Services;
using System;
using System.Threading.Tasks;

namespace PeriodicoUpdate.Services
{
    // Esta clase "implementa" el contrato de la interfaz
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        // Inyectamos el cliente de Cloudinary (que registramos en Program.cs)
        public PhotoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No se proporcionó ningún archivo.");
            }

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                // Opcional: una carpeta para organizar las publicaciones
                Folder = "publicaciones",
                // Opcional: limitamos el tamaño
                Transformation = new Transformation().Width(1200).Height(1200).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Error al subir la imagen: {uploadResult.Error.Message}");
            }

            // Devolvemos nuestro record con los datos
            return new ImageUploadResult(
                Url: uploadResult.SecureUrl.ToString(),
                PublicId: uploadResult.PublicId
            );
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return false; // No hay nada que borrar
            }

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            // "ok" significa que se borró correctamente
            return result.Result == "ok";
        }
    }
}