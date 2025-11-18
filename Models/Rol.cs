namespace PeriodicoUpdate.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public ICollection<UsuarioRol> UsuariosRoles { get; set; }
    }
}
