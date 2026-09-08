namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class CreateReceptionistProfileRequestDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public required string Email { get; set; }

        public required Guid OfficeId { get; set; }
    }
}
