using Application;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Services.API.Application.Exceptions
{
    public class InactiveServiceLinkException : BadRequestException
    {
        public InactiveServiceLinkException(Guid serviceId)
            : base(string.Format(ServiceMessages.InactiveServiceMessage, serviceId))
        {
        }
    }
}
