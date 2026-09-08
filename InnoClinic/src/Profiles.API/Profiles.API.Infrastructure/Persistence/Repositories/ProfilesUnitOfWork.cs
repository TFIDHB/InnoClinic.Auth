using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Profiles.API.Infrastructure.Persistence.Repositories
{
    public class ProfilesUnitOfWork(ProfilesDbContext context, IServiceProvider provider) : IProfilesUnitOfWork, IDisposable
    {
        private IDoctorProfilesRepository? _doctorProfilesRepository;
        private IPatientProfilesRepository? _patientProfilesRepository;
        private IReceptionistProfilesRepository? _receptionistProfilesRepository;

        public IDoctorProfilesRepository DoctorProfilesRepository =>
            _doctorProfilesRepository ??= provider.GetRequiredService<IDoctorProfilesRepository>();

        public IPatientProfilesRepository PatientProfilesRepository =>
            _patientProfilesRepository ??= provider.GetRequiredService<IPatientProfilesRepository>();

        public IReceptionistProfilesRepository ReceptionistProfilesRepository =>
            _receptionistProfilesRepository ??= provider.GetRequiredService<IReceptionistProfilesRepository>();

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);

        public void Dispose() => context.Dispose();
    }
}
