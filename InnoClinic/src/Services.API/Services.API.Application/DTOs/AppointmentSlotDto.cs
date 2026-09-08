namespace InnoClinic.Services.API.Application.DTOs
{
    public class AppointmentSlotDto
    {
        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
