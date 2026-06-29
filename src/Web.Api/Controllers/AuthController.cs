using Kessler.Application.Identity.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class AuthController(ISender sender) : ApiControllerBase
{
    /// <summary>Login do painel administrativo (a artista).</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));
}
