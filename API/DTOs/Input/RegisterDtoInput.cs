namespace API.DTOs.Input
{
    public class RegisterDtoInput
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
