namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class UpdateResultRequestDto
    {
        public required string Complaints { get; set; }

        public required string Conclusion { get; set; }

        public required string Recommendations { get; set; }
    }
}
