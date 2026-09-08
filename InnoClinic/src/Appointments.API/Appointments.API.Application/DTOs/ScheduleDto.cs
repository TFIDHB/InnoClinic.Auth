namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class ScheduleDto
    {
        public required Guid AppointmentId { get; set; }

        public required TimeOnly StartTime { get; set; }

        public required TimeOnly EndTime { get; set; }

        public required Guid PatientId { get; set; }

        public string PatientFullName { get; set; } = "Unknown patient";

        public string ServiceName { get; set; } = "Unknown service";

        public required bool IsApproved { get; set; }

        public required bool HasResult { get; set; }
    }
}
