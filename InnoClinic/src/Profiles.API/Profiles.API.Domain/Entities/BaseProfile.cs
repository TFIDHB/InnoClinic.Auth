namespace InnoClinic.Profiles.API.Domain.Entities
{
    public class BaseProfile
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public Guid? AccountId { get; set; }
    }
}
