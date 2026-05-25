namespace CleanArch.Domain.Exceptions;

public sealed class NotFoundException(string entity, object id)
    : Exception($"{entity} with id '{id}' was not found.");
