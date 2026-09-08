using InnoClinic.Profiles.API.Domain.Enums;

namespace InnoClinic.Profiles.API.Domain.Entities
{
    public class DoctorProfile : BaseProfile
    {
        public DateOnly? DateOfBirth { get; set; }

        public Guid SpecializationId { get; set; }

        public Guid OfficeId { get; set; }

        public int CareerStartYear { get; set; }

        public DoctorStatus Status { get; set; }
    }
}
