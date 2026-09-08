namespace InnoClinic.Services.API.Application.DTOs
{
    public class CreateSpecializationRequestDto
    {
        public required string Name { get; set; }

        public required IReadOnlyList<Guid> ServiceIds { get; set; }
    }
}
