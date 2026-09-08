namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class AppointmentHistoryItemDto
    {
        public required Guid AppointmentId { get; set; }

        public required DateOnly Date { get; set; }

        public required TimeOnly StartTime { get; set; }

        public required TimeOnly EndTime { get; set; }

        public string DoctorFullName { get; set; } = "Unknown doctor";

        public string ServiceName { get; set; } = "Unknown service";

        public bool HasResult { get; set; }

        public bool CanReschedule { get; set; }
    }
}
