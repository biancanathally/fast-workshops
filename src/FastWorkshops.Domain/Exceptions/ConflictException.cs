namespace FastWorkshops.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há um conflito de dados, por exemplo, ao tentar criar um registro que já existe.
/// </summary>
public class ConflictException(string mensagem) : DomainException(mensagem);
