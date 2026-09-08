using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Auth.API.BLL.Exceptions
{
    public class EmailAlreadyExistsException : BadRequestException
    {
        public EmailAlreadyExistsException()
            : base(BllMessages.EmailExistsMessage)
        {
        }
    }
}
