using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface IServicesUnitOfWork : IBasicUnitOfWork
    {
        IServicesRepository ServicesRepository { get; }

        ISpecializationsRepository SpecializationsRepository { get; }
    }
}
