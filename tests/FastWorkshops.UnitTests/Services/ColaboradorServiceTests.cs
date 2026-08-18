using FastWorkshops.Application.Services;
using FastWorkshops.Domain.Abstractions;
using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace FastWorkshops.UnitTests.Services;

public class ColaboradorServiceTests
{
    private readonly IColaboradorRepository _colaboradores = Substitute.For<IColaboradorRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private ColaboradorService CriarSut() => new(_colaboradores, _uow);

    [Fact]
    public async Task ListarAsync_deve_ordenar_nomes_acentuados_corretamente_em_pt_br()
    {
        // Ordinal (padrão do .NET sem CultureInfo) colocaria "Álvaro" DEPOIS de "Z",
        // porque 'Á' tem código Unicode maior que letras não acentuadas.
        var desordenados = new List<Colaborador>
        {
            new() { Id = 1, Nome = "Bruno" },
            new() { Id = 2, Nome = "Álvaro" },
            new() { Id = 3, Nome = "Ana" },
        };
        _colaboradores.ListarComAtasAsync(Arg.Any<CancellationToken>()).Returns(desordenados);

        var resultado = await CriarSut().ListarAsync(CancellationToken.None);

        resultado.Select(c => c.Nome).Should().ContainInOrder("Álvaro", "Ana", "Bruno");
    }

    [Fact]
    public async Task CriarAsync_deve_remover_espacos_nas_extremidades_do_nome()
    {
        var resultado = await CriarSut().CriarAsync(
            new FastWorkshops.Application.DTOs.CriarColaboradorRequest("  Ana Souza  "));

        resultado.Nome.Should().Be("Ana Souza");
    }
}
