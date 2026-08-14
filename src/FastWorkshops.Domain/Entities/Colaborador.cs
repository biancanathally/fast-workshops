namespace FastWorkshops.Domain.Entities;

public class Colaborador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Navegação: atas em que este colaborador esteve presente
    public List<Ata> Atas { get; set; } = new();
}
