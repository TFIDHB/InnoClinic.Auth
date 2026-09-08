using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Appointments.API.Application.Exceptions
{
    public class OverlappingAppointmentException : BadRequestException
    {
        public OverlappingAppointmentException()
            : base(AppointmentsApiMessages.OverlappingAppointmentMessage)
        {
        }
    }
}
