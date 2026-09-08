using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Services.API.Infrastructure.Persistence.Repositories
{
    public class ServicesUnitOfWork(ServicesDbContext context, IServiceProvider provider) : IServicesUnitOfWork, IDisposable
    {
        private IServicesRepository? _servicesRepository;
        private ISpecializationsRepository? _specializationsRepository;

        public IServicesRepository ServicesRepository =>
            _servicesRepository ??= provider.GetRequiredService<IServicesRepository>();

        public ISpecializationsRepository SpecializationsRepository =>
            _specializationsRepository ??= provider.GetRequiredService<ISpecializationsRepository>();

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);

        public void Dispose() => context.Dispose();
    }
}
