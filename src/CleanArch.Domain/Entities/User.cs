using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // EF Core

    private User(string name, Email email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public static User Create(string name, string email, string passwordHash)
    {
        var user = new User(name, Email.Create(email), passwordHash);
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value));
        return user;
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
