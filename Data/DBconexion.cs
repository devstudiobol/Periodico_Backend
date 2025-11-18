using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PeriodicoUpdate.Models;

namespace PeriodicoUpdate.Data
{
    public class DBconexion : DbContext
    {
        public DBconexion(DbContextOptions<DBconexion> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Detalle_Permiso> DetallePermisos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }

    }
}
