using CleanArch.Application.Common;
using CleanArch.Application.Services;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Repositories;
using MediatR;

namespace CleanArch.Application.Commands.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher
) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await userRepository.ExistsAsync(request.Email, ct))
            return Result.Failure<Guid>($"Email '{request.Email}' is already registered.");

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(user.Id);
    }
}
