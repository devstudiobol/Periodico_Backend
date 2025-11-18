using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PeriodicoUpdate.Data;
using PeriodicoUpdate.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeriodicoUpdate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DBconexion _context;
        private IConfiguration _configuration;

        public UsuariosController(DBconexion context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ListarUsuariosActivos")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListarUsuariosActivos()
        {
            // Filtrar con estado "Activo"
            var UsuarioActivos = await _context.Usuarios
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return UsuarioActivos;
        }


        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        //Login

        [HttpPost("Login")]

        public async Task<IActionResult> Login(string nombreUsuario, string password)
        {
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Password == password);
            if (usuario == null)
                return BadRequest(new { message = "Credenciales invalidas" });

            string jwtToken = GenerarToken(usuario);
            // Devolvemos el token, el ID y el nombre del usuario
            return Ok(new { token = jwtToken, id = usuario.Id, nombre = usuario.Nombre });
        }

        private string GenerarToken(Usuario usuario)
        {
            var Myclaims = new[]
            {
         new Claim(ClaimTypes.Name, usuario.NombreUsuario)
     };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(
                claims: Myclaims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credenciales
            );
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }


        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(int id, string nombre, int telefono, string nombreusuario, string password)
        {
            // Busca la persona por su ID
            var usuarioActual = await _context.Usuarios.FindAsync(id);

            if (usuarioActual == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            usuarioActual.Nombre = nombre;
            usuarioActual.Telefono = telefono;
            usuarioActual.NombreUsuario = nombreusuario;
            usuarioActual.Password = password;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(usuarioActual);
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearUsuario(string nombre, int telefono, string nombreusuario, string password)
        {

            Usuario usuario = new Usuario()
            {
                Nombre = nombre,
                Telefono = telefono,
                NombreUsuario = nombreusuario,
                Password = password,
                Estado = "Activo"
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            usuario.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
