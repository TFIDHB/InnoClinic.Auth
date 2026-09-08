namespace InnoClinic.Appointments.API.Domain.Entities
{
    public class Appointment
    {
        public required Guid Id { get; set; }

        public required Guid PatientId { get; set; }

        public required Guid SpecializationId { get; set; }

        public required Guid DoctorId { get; set; }

        public required Guid ServiceId { get; set; }

        public required Guid OfficeId { get; set; }

        public required DateOnly Date { get; set; }

        public required TimeOnly Time { get; set; }

        public required TimeSpan Duration { get; set; }

        public DateTime StartDateTime => Date.ToDateTime(Time);

        public DateTime EndDateTime => StartDateTime.Add(Duration);

        public bool IsApproved { get; set; }

        public required DateTime CreatedAt { get; set; }
    }
}
