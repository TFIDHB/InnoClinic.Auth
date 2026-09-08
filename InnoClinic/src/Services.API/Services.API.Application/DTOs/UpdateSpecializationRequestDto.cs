namespace InnoClinic.Services.API.Application.DTOs
{
    public class UpdateSpecializationRequestDto
    {
        public required string Name { get; set; }

        public required IReadOnlyList<Guid> ServiceIds { get; set; }
    }
}
