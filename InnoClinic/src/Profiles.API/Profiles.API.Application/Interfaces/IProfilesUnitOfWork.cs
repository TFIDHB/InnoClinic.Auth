using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IProfilesUnitOfWork : IBasicUnitOfWork
    {
        IDoctorProfilesRepository DoctorProfilesRepository { get; }

        IPatientProfilesRepository PatientProfilesRepository { get; }

        IReceptionistProfilesRepository ReceptionistProfilesRepository { get; }
    }
}
