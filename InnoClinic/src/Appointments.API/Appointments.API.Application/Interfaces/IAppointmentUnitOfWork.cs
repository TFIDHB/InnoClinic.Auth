using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IAppointmentUnitOfWork : IBasicUnitOfWork
    {
        IAppointmentRepository AppointmentRepository { get; }

        IResultRepository ResultRepository { get; }
    }
}
