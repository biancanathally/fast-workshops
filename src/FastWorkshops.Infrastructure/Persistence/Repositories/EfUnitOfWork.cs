using FastWorkshops.Domain.Abstractions;
using FastWorkshops.Domain.Exceptions;
using FastWorkshops.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FastWorkshops.Infrastructure.Persistence.Repositories;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    private const int ViolacaoIndiceUnico = 2601;
    private const int ViolacaoChaveUnica  = 2627;

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            return await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
               sql.Number is ViolacaoIndiceUnico or ViolacaoChaveUnica)
        {
            throw new ConflictException(
                "A operação viola uma restrição de unicidade do banco de dados.");
        }
    }
}
