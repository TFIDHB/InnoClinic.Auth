namespace InnoClinic.Services.API.Application.DTOs
{
    public class GetAvailableDatesRequestDto
    {
        public Guid ServiceId { get; set; }

        public Guid? DoctorId { get; set; }
    }
}
