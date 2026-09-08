using InnoClinic.Profiles.API.Domain.Enums;

namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class UpdateDoctorProfileRequestDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        /// <example>a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1.</example>
        public Guid SpecializationId { get; set; }

        public Guid OfficeId { get; set; }

        public int CareerStartYear { get; set; }

        public DoctorStatus Status { get; set; }
    }
}
