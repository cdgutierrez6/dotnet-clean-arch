using CleanArch.Application.Common;
using MediatR;

namespace CleanArch.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<Result<Guid>>;
