using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Profiles.API.Application.Exceptions
{
    public class ProfileAlreadyExistsException : BadRequestException
    {
        public ProfileAlreadyExistsException()
            : base(ProfilesApplicationMessages.ProfileAlreadyExistsMessage)
        {
        }
    }
}
