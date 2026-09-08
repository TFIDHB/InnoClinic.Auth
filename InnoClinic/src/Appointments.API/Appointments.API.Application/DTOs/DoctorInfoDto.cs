namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class DoctorInfoDto
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public required Guid SpecializationId { get; set; }

        public required Guid OfficeId { get; set; }

        public required Guid AccountId { get; set; }

        public int Status { get; set; }
    }
}
