namespace InnoClinic.Services.API.Application.DTOs
{
    public class CreateServiceRequestDto
    {
        public required string Name { get; set; }

        public double Price { get; set; }

        /// <example>a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1.</example>
        public Guid ServiceCategoryId { get; set; }

        public Guid? SpecializationId { get; set; }
    }
}
