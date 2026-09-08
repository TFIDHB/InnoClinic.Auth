using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence.Repositories
{
    public class AppointmentUnitOfWork(AppointmentDbContext context, IServiceProvider provider)
        : IAppointmentUnitOfWork, IDisposable
    {
        private IAppointmentRepository? _appointmentRepository;
        private IResultRepository? _resultRepository;

        public IAppointmentRepository AppointmentRepository =>
            _appointmentRepository ??= provider.GetRequiredService<IAppointmentRepository>();

        public IResultRepository ResultRepository =>
            _resultRepository ??= provider.GetRequiredService<IResultRepository>();

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);

        public void Dispose() => context.Dispose();
    }
}
