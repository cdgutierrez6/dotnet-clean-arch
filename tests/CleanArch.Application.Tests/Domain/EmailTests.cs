using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Application.Tests.Domain;

public sealed class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    [InlineData("user.name+tag@domain.co")]
    public void Create_WithValidEmail_ShouldSucceed(string email)
    {
        var result = Email.Create(email);

        result.Value.Should().Be(email.ToLowerInvariant().Trim());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("@nodomain.com")]
    [InlineData("user@")]
    public void Create_WithInvalidEmail_ShouldThrowDomainException(string email)
    {
        var act = () => Email.Create(email);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoEmails_WithSameValue_ShouldBeEqual()
    {
        var email1 = Email.Create("user@example.com");
        var email2 = Email.Create("USER@EXAMPLE.COM");

        email1.Should().Be(email2);
    }
}
