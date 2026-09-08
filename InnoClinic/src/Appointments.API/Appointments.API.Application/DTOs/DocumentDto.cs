namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class DocumentDto
    {
        public required Guid Id { get; set; }

        public required string Url { get; set; }

        public required Guid ResultId { get; set; }
    }
}
