using Application;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Services.API.Application.Exceptions
{
    public class ServicesNotFoundException : NotFoundException
    {
        public ServicesNotFoundException(string missingIds)
            : base(string.Format(ServiceMessages.ServicesNotFoundMessage, missingIds))
        {
        }
    }
}
