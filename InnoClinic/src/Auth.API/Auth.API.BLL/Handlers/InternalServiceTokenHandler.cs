using InnoClinic.Shared.Generators;
using InnoClinic.Shared.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace InnoClinic.Auth.API.BLL.Handlers
{
    public class InternalServiceTokenHandler(IOptions<JwtSettings> jwtSettings) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = InternalServiceTokenGenerator.Generate(jwtSettings.Value);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return base.SendAsync(request, ct);
        }
    }
}
