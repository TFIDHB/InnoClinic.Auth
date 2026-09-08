using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Auth.API.BLL.Exceptions
{
    public class UserNotFoundException : BadRequestException
    {
        public UserNotFoundException()
            : base(BllMessages.UserNotFoundMessage)
        {
        }
    }
}
