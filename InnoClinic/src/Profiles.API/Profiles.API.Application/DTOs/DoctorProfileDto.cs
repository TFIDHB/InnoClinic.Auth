using InnoClinic.Profiles.API.Domain.Enums;

namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class DoctorProfileDto
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Guid AccountId { get; set; }

        public Guid SpecializationId { get; set; }

        public Guid OfficeId { get; set; }

        public int CareerStartYear { get; set; }

        public DoctorStatus Status { get; set; }

        public string? TemporaryPassword { get; set; }
    }
}
