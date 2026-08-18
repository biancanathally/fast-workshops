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
}
