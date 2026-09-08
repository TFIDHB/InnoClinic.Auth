namespace InnoClinic.Services.API.Application.DTOs
{
    public class ServiceDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public double Price { get; set; }

        public Guid ServiceCategoryId { get; set; }

        public Guid SpecializationId { get; set; }

        public bool IsActive { get; set; }
    }
}
