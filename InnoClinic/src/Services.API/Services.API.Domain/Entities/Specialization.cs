namespace InnoClinic.Services.API.Domain.Entities
{
    public class Specialization
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required bool IsActive { get; set; }

        public virtual ICollection<Service> Services { get; set; } = [];
    }
}
