using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Auth.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController(IAuthService authService) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccountInfo(Guid id, CancellationToken ct = default)
        {
            var result = await authService.GetUserAccountInfo(id, User, ct);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAccountInfo(Guid id, [FromBody] UpdateUserAccountInfoDto dto, CancellationToken ct = default)
        {
            await authService.UpdateUserAccountInfo(id, dto, User, ct);
            return Ok();
        }
    }
}
