namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class RescheduleAppointmentRequestDto
    {
        public required Guid DoctorId { get; set; }

        public required DateOnly Date { get; set; }

        public required TimeOnly Time { get; set; }
    }
}
