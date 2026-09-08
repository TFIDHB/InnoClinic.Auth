namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IPatientFields
    {
        string FirstName { get; }

        string LastName { get; }

        string? MiddleName { get; }

        DateOnly? DateOfBirth { get; }
    }
}
