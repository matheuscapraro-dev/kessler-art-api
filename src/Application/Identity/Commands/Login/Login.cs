using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions.Authentication;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Identity.Commands.Login;

public sealed record AuthResultDto(
    string Token,
    DateTime ExpiresAtUtc,
    string Name,
    string Email,
    string Role);

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResultDto>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class LoginCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    public async Task<Result<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return AppResults.Unauthorized("E-mail ou senha inválidos.");

        var (token, expiresAt) = tokenGenerator.Generate(user);
        return Result.Ok(new AuthResultDto(token, expiresAt, user.Name, user.Email, user.Role));
    }
}
