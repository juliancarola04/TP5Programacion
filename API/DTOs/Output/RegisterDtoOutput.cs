namespace API.DTOs.Output
{
    public class RegisterDtoOutput
    {
        public bool Exito { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiracion { get; set; }
    }
}
