namespace FastWorkshops.Domain.Exceptions;

public class NotFoundException(string recurso, object id)
    : DomainException($"{recurso} com id {id} não foi encontrado.");
