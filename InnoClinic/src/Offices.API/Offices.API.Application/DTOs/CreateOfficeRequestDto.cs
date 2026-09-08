namespace InnoClinic.Offices.API.Application.DTOs
{
    public class CreateOfficeRequestDto
    {
        public required string Address { get; set; }

        public Guid PhotoId { get; set; }

        public required string RegistryPhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
