namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class ReceptionistProfileDto
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public Guid AccountId { get; set; }

        public Guid OfficeId { get; set; }

        public string? TemporaryPassword { get; set; }
    }
}
