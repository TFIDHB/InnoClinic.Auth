using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Profiles.API.Controllers
{
    [Authorize(Roles = Roles.InternalService)]
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountSearchController(IAccountSearchService accountSearchService) : ControllerBase
    {
        [HttpGet("{id}/profile-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountProfileInfoDto>> GetProfileInfo(Guid id, CancellationToken ct = default)
        {
            var result = await accountSearchService.GetByAccountIdAsync(id, ct);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
