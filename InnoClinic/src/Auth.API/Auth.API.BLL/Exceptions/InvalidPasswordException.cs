using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Auth.API.BLL.Exceptions
{
    public class InvalidPasswordException : BadRequestException
    {
        public InvalidPasswordException()
            : base(BllMessages.InvalidPasswordMessage)
        {
        }
    }
}
