using InnoClinic.Auth.API.DAL.Entities;

namespace InnoClinic.Auth.API.BLL.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, string role);

        string GenerateRefreshToken();
    }
}
