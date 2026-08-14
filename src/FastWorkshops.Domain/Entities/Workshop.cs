namespace FastWorkshops.Domain.Entities;

public class Workshop
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataRealizacao { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Um workshop tem no máximo uma ata de presença
    public Ata? Ata { get; set; }
}
