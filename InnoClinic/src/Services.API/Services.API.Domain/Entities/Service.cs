namespace InnoClinic.Services.API.Domain.Entities
{
    public class Service
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required double Price { get; set; }

        public required Guid ServiceCategoryId { get; set; }

        public required ServiceCategory ServiceCategory { get; set; }

        public Guid? SpecializationId { get; set; }

        public Specialization? Specialization { get; set; }

        public required bool IsActive { get; set; }
    }
}
