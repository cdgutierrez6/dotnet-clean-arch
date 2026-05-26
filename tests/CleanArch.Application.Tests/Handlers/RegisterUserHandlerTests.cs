using CleanArch.Application.Commands.RegisterUser;
using CleanArch.Application.Services;
using CleanArch.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArch.Application.Tests.Handlers;

public sealed class RegisterUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
        _handler = new RegisterUserHandler(_userRepo.Object, _unitOfWork.Object, _hasher.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        _userRepo.Setup(r => r.ExistsAsync(It.IsAny<string>(), default)).ReturnsAsync(false);

        var command = new RegisterUserCommand("Cristian", "cristian@example.com", "Password1");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _userRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>(), default), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnFailure()
    {
        _userRepo.Setup(r => r.ExistsAsync("existing@example.com", default)).ReturnsAsync(true);

        var command = new RegisterUserCommand("Cristian", "existing@example.com", "Password1");
        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already registered");

        _userRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>(), default), Times.Never);
    }
}
