using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeriodicoUpdate.Data;
using PeriodicoUpdate.Models;

namespace PeriodicoUpdate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Detalle_PermisosController : ControllerBase
    {
        private readonly DBconexion _context;

        public Detalle_PermisosController(DBconexion context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("ListarDetallePermisosActivos")]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> ListarDetallePermisosActivos()
        {
            // Filtrar con estado "Activo"
            var DetallePermisosActivos = await _context.DetallePermisos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return DetallePermisosActivos;
        }

        // GET: api/Detalle_Permisos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> GetDetallePermisos()
        {
            return await _context.DetallePermisos.ToListAsync();
        }

        // GET: api/Detalle_Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detalle_Permiso>> GetDetalle_Permiso(int id)
        {
            var detalle_Permiso = await _context.DetallePermisos.FindAsync(id);

            if (detalle_Permiso == null)
            {
                return NotFound();
            }

            return detalle_Permiso;
        }

        // PUT: api/Detalle_Permisos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(int id, int idpermiso, int idusuario)
        {
            // Busca la persona por su ID
            var DetallePermisosActual = await _context.DetallePermisos.FindAsync(id);

            if (DetallePermisosActual == null)
            {
                return NotFound("El detalle permisos no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            DetallePermisosActual.idpermiso = idpermiso;
            DetallePermisosActual.idusuario = idusuario;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(DetallePermisosActual);
        }

        // POST: api/Detalle_Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearDetallePermiso(int idpermiso, int idusuario)
        {

            Detalle_Permiso detallepermiso = new Detalle_Permiso()
            {
                idusuario = idusuario,
                idpermiso = idpermiso,
                Estado = "Activo"
            };

            await _context.DetallePermisos.AddAsync(detallepermiso);
            await _context.SaveChangesAsync();

            return Ok(detallepermiso);
        }

        // DELETE: api/Detalle_Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallePermiso(int id)
        {
            var DetallePermiso = await _context.DetallePermisos.FindAsync(id);

            if (DetallePermiso == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            DetallePermiso.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
