namespace InnoClinic.Offices.API.Application.DTOs
{
    public class OfficeDto
    {
        public Guid Id { get; set; }

        public required string Address { get; set; }

        public Guid PhotoId { get; set; }

        public required string RegistryPhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
