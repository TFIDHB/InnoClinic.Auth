namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class AppointmentSlotDto
    {
        public required DateOnly Date { get; set; }

        public required TimeOnly Time { get; set; }

        public required TimeSpan Duration { get; set; }
    }
}
