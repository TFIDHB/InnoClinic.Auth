namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class CreateAppointmentRequestDto
    {
        public required Guid PatientId { get; set; }

        public required Guid SpecializationId { get; set; }

        public required Guid DoctorId { get; set; }

        public required Guid ServiceId { get; set; }

        public required Guid OfficeId { get; set; }

        /// <example>2026-05-20.</example>
        public required DateOnly Date { get; set; }

        /// <example>14:30.</example>
        public required TimeOnly Time { get; set; }
    }
}
