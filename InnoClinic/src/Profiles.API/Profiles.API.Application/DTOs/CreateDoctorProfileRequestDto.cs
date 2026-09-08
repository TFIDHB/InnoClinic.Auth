using InnoClinic.Profiles.API.Domain.Enums;

namespace InnoClinic.Profiles.API.Application.DTOs
{
    public class CreateDoctorProfileRequestDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public required string Email { get; set; }

        /// <example>a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1.</example>
        public required Guid SpecializationId { get; set; }

        public required Guid OfficeId { get; set; }

        public required int CareerStartYear { get; set; }

        public required DoctorStatus Status { get; set; }
    }
}
