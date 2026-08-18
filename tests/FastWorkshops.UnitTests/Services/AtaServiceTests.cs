using FastWorkshops.Application.DTOs;
using FastWorkshops.Application.Services;
using FastWorkshops.Domain.Abstractions;
using FastWorkshops.Domain.Entities;
using FastWorkshops.Domain.Exceptions;
using FastWorkshops.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace FastWorkshops.UnitTests.Services;

public class AtaServiceTests
{
    private readonly IAtaRepository _atas = Substitute.For<IAtaRepository>();
    private readonly IWorkshopRepository _workshops = Substitute.For<IWorkshopRepository>();
    private readonly IColaboradorRepository _colaboradores = Substitute.For<IColaboradorRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private AtaService CriarSut() => new(_atas, _workshops, _colaboradores, _uow);

    // ---------- CriarAsync ----------

    [Fact]
    public async Task CriarAsync_deve_lancar_NotFound_quando_workshop_nao_existe()
    {
        _workshops.ExisteAsync(99, Arg.Any<CancellationToken>()).Returns(false);

        var acao = () => CriarSut().CriarAsync(new CriarAtaRequest(99, null));

        await acao.Should().ThrowAsync<NotFoundException>();
        await _uow.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsync_deve_lancar_Conflict_quando_workshop_ja_possui_ata()
    {
        _workshops.ExisteAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _atas.ExistePorWorkshopAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var acao = () => CriarSut().CriarAsync(new CriarAtaRequest(1, null));

        await acao.Should().ThrowAsync<ConflictException>();
        await _uow.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsync_deve_lancar_NotFound_listando_colaboradores_inexistentes()
    {
        _workshops.ExisteAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _atas.ExistePorWorkshopAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        _colaboradores.ObterPorIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns([new Colaborador { Id = 1, Nome = "Ana" }]);

        var acao = () => CriarSut().CriarAsync(new CriarAtaRequest(1, [1, 999]));

        var excecao = await acao.Should().ThrowAsync<NotFoundException>();
        excecao.Which.Message.Should().Contain("999");
    }

    [Fact]
    public async Task CriarAsync_deve_ignorar_ids_duplicados_na_requisicao()
    {
        _workshops.ExisteAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _atas.ExistePorWorkshopAsync(1, Arg.Any<CancellationToken>()).Returns(false);

        var colaborador = new Colaborador { Id = 1, Nome = "Ana" };
        _colaboradores.ObterPorIdsAsync(
                Arg.Is<IEnumerable<int>>(ids => ids.Count() == 1), // deduplicado antes de chegar aqui
                Arg.Any<CancellationToken>())
            .Returns([colaborador]);

        var workshop = new Workshop { Id = 1, Nome = "W", Descricao = "D" };
        var ataCriada = new Ata { Id = 10, WorkshopId = 1, Workshop = workshop, Colaboradores = [colaborador] };
        _atas.ObterCompletaAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(ataCriada);

        await CriarSut().CriarAsync(new CriarAtaRequest(1, [1, 1, 1]));

        await _colaboradores.Received(1).ObterPorIdsAsync(
            Arg.Is<IEnumerable<int>>(ids => ids.Count() == 1), Arg.Any<CancellationToken>());
    }

    // ---------- AdicionarColaboradorAsync ----------

    [Fact]
    public async Task AdicionarColaboradorAsync_deve_ser_idempotente()
    {
        var colaborador = new Colaborador { Id = 5, Nome = "Bruno" };
        var ata = new Ata { Id = 1, Colaboradores = [colaborador] }; // já contém o colaborador
        _atas.ObterPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ata);

        await CriarSut().AdicionarColaboradorAsync(1, 5, CancellationToken.None);

        // Não deve buscar o colaborador de novo nem commitar — early return
        await _colaboradores.DidNotReceive().ObterPorIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _uow.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AdicionarColaboradorAsync_deve_lancar_NotFound_quando_ata_nao_existe()
    {
        _atas.ObterPorIdAsync(1, Arg.Any<CancellationToken>()).Returns((Ata?)null);

        var acao = () => CriarSut().AdicionarColaboradorAsync(1, 5, CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>();
    }

    // ---------- RemoverColaboradorAsync ----------

    [Fact]
    public async Task RemoverColaboradorAsync_deve_lancar_NotFound_quando_colaborador_nao_esta_na_ata()
    {
        var ata = new Ata { Id = 1, Colaboradores = [] };
        _atas.ObterPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ata);

        var acao = () => CriarSut().RemoverColaboradorAsync(1, 5, CancellationToken.None);

        await acao.Should().ThrowAsync<NotFoundException>();
    }
}
