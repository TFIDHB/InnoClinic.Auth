using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Auth.API.BLL.Exceptions
{
    public class InactiveEntityException : BadRequestException
    {
        public InactiveEntityException()
            : base(BllMessages.InactiveEntityMessage)
        {
        }
    }
}
