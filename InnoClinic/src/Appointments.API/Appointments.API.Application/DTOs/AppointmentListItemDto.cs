namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class AppointmentListItemDto
    {
        public required Guid AppointmentId { get; set; }

        public required TimeOnly StartTime { get; set; }

        public required TimeOnly EndTime { get; set; }

        public string DoctorFullName { get; set; } = "Unknown doctor";

        public string PatientFullName { get; set; } = "Unknown patient";

        public string? PatientPhoneNumber { get; set; }

        public string ServiceName { get; set; } = "Unknown service";

        public required bool IsApproved { get; set; }
    }
}
