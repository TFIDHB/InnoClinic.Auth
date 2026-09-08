namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class PatientInfoDto
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public required Guid AccountId { get; set; }
    }
}
