namespace FastWorkshops.Domain.Exceptions;

public class NotFoundException(string recurso, object id)
    : Exception($"{recurso} com id {id} não foi encontrado.");
