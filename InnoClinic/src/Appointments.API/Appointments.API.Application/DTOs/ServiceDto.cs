namespace InnoClinic.Appointments.API.Application.DTOs
{
    public class ServiceDto
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required double Price { get; set; }

        public required Guid ServiceCategoryId { get; set; }

        public required Guid SpecializationId { get; set; }

        public required bool IsActive { get; set; }
    }
}
