namespace API.DTOs.Output
{
    public class UsuarioDtoOutput
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required bool EsAdministrador { get; set; }
    }
}
