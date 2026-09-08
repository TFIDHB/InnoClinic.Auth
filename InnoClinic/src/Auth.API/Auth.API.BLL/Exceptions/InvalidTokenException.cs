using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Auth.API.BLL.Exceptions
{
    public class InvalidTokenException : BadRequestException
    {
        public InvalidTokenException()
            : base(BllMessages.InvalidTokenMessage)
        {
        }
    }
}
