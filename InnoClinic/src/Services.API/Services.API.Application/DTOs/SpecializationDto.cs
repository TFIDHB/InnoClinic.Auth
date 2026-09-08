namespace InnoClinic.Services.API.Application.DTOs
{
    public class SpecializationDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public bool IsActive { get; set; }

        public required IReadOnlyList<ServiceDto> Services { get; set; }
    }
}
