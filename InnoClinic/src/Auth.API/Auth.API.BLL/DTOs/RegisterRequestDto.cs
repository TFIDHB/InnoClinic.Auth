namespace InnoClinic.Auth.API.BLL.DTOs
{
    public class RegisterRequestDto
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
