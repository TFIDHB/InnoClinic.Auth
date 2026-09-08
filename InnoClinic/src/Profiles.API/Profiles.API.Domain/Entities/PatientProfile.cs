namespace InnoClinic.Profiles.API.Domain.Entities
{
    public class PatientProfile : BaseProfile
    {
        public DateOnly? DateOfBirth { get; set; }

        public bool IsLinkedToAccount { get; set; }
    }
}
