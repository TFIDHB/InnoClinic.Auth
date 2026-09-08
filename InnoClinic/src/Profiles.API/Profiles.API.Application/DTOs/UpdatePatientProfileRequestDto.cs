namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class UpdatePatientProfileRequestDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public required string PhoneNumber { get; set; }
    }
}
