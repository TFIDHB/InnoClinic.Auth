namespace InnoClinic.Services.API.Domain.Entities
{
    public class ServiceCategory
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int TimeSlotSize { get; set; }
    }
}
