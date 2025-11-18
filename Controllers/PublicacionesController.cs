using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeriodicoUpdate.Data;
using PeriodicoUpdate.Models;
using PeriodicoUpdate.Services;

namespace PeriodicoUpdate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacionesController : ControllerBase
    {
        private readonly DBconexion _context;
        private readonly IPhotoService _photoService;

        public PublicacionesController(DBconexion context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        [HttpGet]
        [Route("ListarPorCategoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Publicacion>>> ListarPorCategoria(int idCategoria, [FromQuery] string estado = "Activo")
        {
            var query = _context.Publicaciones
                .Where(p => p.idcategoria == idCategoria);

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            var publicaciones = await query
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            if (!publicaciones.Any())
            {
                return NotFound("No se encontraron publicaciones para los criterios especificados");
            }

            return publicaciones;
        }

        [HttpGet]
        [Route("ListarPublicacionesActivos")]
        public async Task<ActionResult<IEnumerable<Publicacion>>> ListarPublicacionActivos()
        {
            return await _context.Publicaciones
                .Where(e => e.Estado == "Activo")
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publicacion>>> GetPublicaciones()
        {
            return await _context.Publicaciones
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Publicacion>> GetPublicacion(int id)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return NotFound();

            // Incrementar el contador de visualizaciones
            publicacion.Visualizacion += 1;
            await _context.SaveChangesAsync();

            return publicacion;
        }

        [HttpPut]
        [Route("Actualizar/{id}")]
        public async Task<IActionResult> ActualizarPublicacion(int id, [FromForm] PublicacionUpdateDto publicacionUpdate)
        {
            var publicacionActual = await _context.Publicaciones.FindAsync(id);
            if (publicacionActual == null) return NotFound("La publicación no fue encontrada.");

            // Actualizar campos de texto
            publicacionActual.Titulo = publicacionUpdate.Titulo;
            publicacionActual.Descripcion = publicacionUpdate.Descripcion;
            publicacionActual.Fecha = publicacionUpdate.Fecha;
            publicacionActual.idcategoria = publicacionUpdate.idcategoria;

            // Manejar imagen si se proporciona
            if (publicacionUpdate.Imagen != null)
            {
                // Eliminar imagen anterior de Cloudinary si existe
                if (!string.IsNullOrEmpty(publicacionActual.ImagenPublicId))
                {
                    await _photoService.DeletePhotoAsync(publicacionActual.ImagenPublicId);
                }

                // Guardar nueva imagen en Cloudinary
                var uploadResult = await _photoService.AddPhotoAsync(publicacionUpdate.Imagen);

                // Actualizar los campos en el modelo
                publicacionActual.ImagenUrl = uploadResult.Url;
                publicacionActual.ImagenPublicId = uploadResult.PublicId;
            }

            await _context.SaveChangesAsync();
            return Ok(publicacionActual);
        }

        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearPublicacion([FromForm] PublicacionCreateDto publicacionCreate)
        {
            var publicacion = new Publicacion()
            {
                Titulo = publicacionCreate.Titulo,
                Descripcion = publicacionCreate.Descripcion,
                Fecha = publicacionCreate.Fecha,
                Visualizacion = 0,
                idusuario = publicacionCreate.idusuario,
                idcategoria = publicacionCreate.idcategoria,
                Estado = "Activo"
            };

            if (publicacionCreate.Imagen != null)
            {
                // Subir la imagen a Cloudinary
                var uploadResult = await _photoService.AddPhotoAsync(publicacionCreate.Imagen);

                // Guardar los datos de Cloudinary en el modelo
                publicacion.ImagenUrl = uploadResult.Url;
                publicacion.ImagenPublicId = uploadResult.PublicId;
            }

            await _context.Publicaciones.AddAsync(publicacion);
            await _context.SaveChangesAsync();
            // Retornar la publicación creada (mejor práctica que solo Ok)
            return CreatedAtAction(nameof(GetPublicacion), new { id = publicacion.Id }, publicacion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicacion(int id)
        {
            // Este es un "soft delete" (borrado lógico)
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return NotFound("La publicación no fue encontrada.");

            publicacion.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new { message = "La publicación ha sido desactivada." });
        }

        [HttpDelete("permanente/{id}")]
        public async Task<IActionResult> DeletePublicacionPermanente(int id)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return NotFound("La publicación no fue encontrada.");

            // 1. Borrar de Cloudinary (si tiene imagen)
            if (!string.IsNullOrEmpty(publicacion.ImagenPublicId))
            {
                await _photoService.DeletePhotoAsync(publicacion.ImagenPublicId);
            }

            // 2. Borrar de la Base de Datos
            _context.Publicaciones.Remove(publicacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "La publicación ha sido eliminada permanentemente." });
        }
    }

    // --- DTOs (Data Transfer Objects) ---
    // (Puedes moverlos a una carpeta/archivo 'Dtos' separado)

    public class PublicacionCreateDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int idusuario { get; set; }
        public int idcategoria { get; set; }
        public IFormFile? Imagen { get; set; } // Opcional
    }

    public class PublicacionUpdateDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int idcategoria { get; set; }
        public IFormFile? Imagen { get; set; } // Opcional
    }
}
