namespace InnoClinic.Services.API.Application.DTOs
{
    public class GetAvailableSlotsRequestDto
    {
        public DateOnly Date { get; set; }

        public Guid ServiceId { get; set; }

        public Guid? DoctorId { get; set; }
    }
}
