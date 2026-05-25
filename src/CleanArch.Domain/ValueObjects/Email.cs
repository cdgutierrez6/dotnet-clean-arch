using System.Text.RegularExpressions;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException($"'{value}' is not a valid email address.");

        return new Email(value.ToLowerInvariant().Trim());
    }

    public override string ToString() => Value;
}
