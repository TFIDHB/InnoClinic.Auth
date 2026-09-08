namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class ResultDto
    {
        public required Guid Id { get; set; }

        public required Guid AppointmentId { get; set; }

        public required DateOnly Date { get; set; }

        public string PatientFullName { get; set; } = "Unknown patient";

        public DateOnly? PatientDateOfBirth { get; set; }

        public string DoctorFullName { get; set; } = "Unknown doctor";

        public string DoctorSpecialization { get; set; } = "Unknown specialization";

        public string ServiceName { get; set; } = "Unknown service";

        public required string Complaints { get; set; }

        public required string Conclusion { get; set; }

        public required string Recommendations { get; set; }

        public required bool CanEdit { get; set; }
    }
}
