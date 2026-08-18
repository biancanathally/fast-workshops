using FastWorkshops.Domain.Entities;
using FluentAssertions;

namespace FastWorkshops.UnitTests.Entities;

public class AtaTests
{
    [Fact]
    public void AdicionarColaborador_nao_deve_duplicar_quando_ja_presente()
    {
        var colaborador = new Colaborador { Id = 1, Nome = "Ana" };
        var ata = new Ata { Colaboradores = [colaborador] };

        ata.AdicionarColaborador(colaborador);

        ata.Colaboradores.Should().HaveCount(1);
    }

    [Fact]
    public void AdicionarColaborador_deve_incluir_quando_ainda_nao_presente()
    {
        var colaborador = new Colaborador { Id = 1, Nome = "Ana" };
        var ata = new Ata { Colaboradores = [] };

        ata.AdicionarColaborador(colaborador);

        ata.Colaboradores.Should().ContainSingle(c => c.Id == 1);
    }

    [Fact]
    public void RemoverColaborador_deve_retornar_false_quando_nao_presente()
    {
        var ata = new Ata { Colaboradores = [] };

        var removeu = ata.RemoverColaborador(999);

        removeu.Should().BeFalse();
    }

    [Fact]
    public void RemoverColaborador_deve_retornar_true_e_remover_quando_presente()
    {
        var colaborador = new Colaborador { Id = 1, Nome = "Ana" };
        var ata = new Ata { Colaboradores = [colaborador] };

        var removeu = ata.RemoverColaborador(1);

        removeu.Should().BeTrue();
        ata.Colaboradores.Should().BeEmpty();
    }
}
