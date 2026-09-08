namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class CreateStaffAccountResponseDto
    {
        public required Guid AccountId { get; set; }

        public required string TemporaryPassword { get; set; }
    }
}
