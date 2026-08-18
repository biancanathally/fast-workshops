namespace FastWorkshops.Domain.Exceptions;

/// <summary>
/// Base para exceções que representam violação de uma regra de negócio,
/// como oposto a falhas técnicas/infraestruturais.
/// </summary>
public abstract class DomainException(string message) : Exception(message);
