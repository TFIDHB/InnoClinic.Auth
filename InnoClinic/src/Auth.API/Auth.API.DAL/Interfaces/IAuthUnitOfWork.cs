using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Auth.API.DAL.Interfaces
{
    public interface IAuthUnitOfWork : IBasicUnitOfWork
    {
        IUserRepository UserRepository { get; }
    }
}
