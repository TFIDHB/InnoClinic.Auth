namespace InnoClinic.Auth.API.BLL.DTOs
{
    public class UserAccountInfoDto
    {
        public string? PhoneNumber { get; set; }

        public Guid? PhotoId { get; set; }

        public bool IsEmailVerified { get; set; }
    }
}
