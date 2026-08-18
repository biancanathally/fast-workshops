namespace FastWorkshops.Domain.Entities;

public class Ata
{
    public int Id { get; set; }
    public int WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public List<Colaborador> Colaboradores { get; set; } = new();

    // Token de concorrência otimista: EF Core inclui automaticamente
    // no WHERE do UPDATE e detecta modificação concorrente.
    public byte[] RowVersion { get; set; } = null!;

    /// <summary>
    /// Adiciona um colaborador à ata. Idempotente: se já estiver presente, não faz nada.
    /// </summary>
    public void AdicionarColaborador(Colaborador colaborador)
    {
        if (Colaboradores.Any(c => c.Id == colaborador.Id))
            return;

        Colaboradores.Add(colaborador);
    }

    /// <summary>
    /// Remove um colaborador da ata.
    /// </summary>
    /// <returns>true se removeu; false se o colaborador não estava presente.</returns>
    public bool RemoverColaborador(int colaboradorId)
    {
        var colaborador = Colaboradores.FirstOrDefault(c => c.Id == colaboradorId);
        if (colaborador is null) return false;

        Colaboradores.Remove(colaborador);
        return true;
    }
}
