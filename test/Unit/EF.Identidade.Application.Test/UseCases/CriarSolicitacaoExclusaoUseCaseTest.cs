using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Identidade.Application.DTOs.Requests;
using EF.Identidade.Application.UseCases;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.Identidade.Domain.Models;
using EF.Identidade.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Identidade.Application.Test.UseCases;

public class CriarSolicitacaoExclusaoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ISolicitacaoExclusaoRepository> _solicitacaoExclusaoRepositoryMock;
    private readonly ICriarSolicitacaoExclusaoUseCase _criarSolicitacaoExclusaoUseCase;

    public CriarSolicitacaoExclusaoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _solicitacaoExclusaoRepositoryMock = _fixture.Freeze<Mock<ISolicitacaoExclusaoRepository>>();
        _criarSolicitacaoExclusaoUseCase = _fixture.Create<CriarSolicitacaoExclusaoUseCase>();
    }

    [Fact]
    public async Task DeveCriarSolicitacaoExclusao()
    {
        // Arrange
        var criarSolicitacaoExclusaoDto = _fixture.Create<CriarSolicitacaoExclusaoDto>();

        _solicitacaoExclusaoRepositoryMock.Setup(x => x.BuscarPorClienteId(criarSolicitacaoExclusaoDto.ClienteId!.Value)).ReturnsAsync((SolicitacaoExclusao?)null);
        _solicitacaoExclusaoRepositoryMock.Setup(x => x.Criar(It.IsAny<SolicitacaoExclusao>()));
        _solicitacaoExclusaoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _criarSolicitacaoExclusaoUseCase.Handle(criarSolicitacaoExclusaoDto);

        // Assert
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.BuscarPorClienteId(criarSolicitacaoExclusaoDto.ClienteId!.Value), Times.Once);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.Criar(It.IsAny<SolicitacaoExclusao>()), Times.Once);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCriarSolicitacaoExclusaComClienteInvalido()
    {
        // Arrange
        var criarSolicitacaoExclusaoDto = _fixture.Build<CriarSolicitacaoExclusaoDto>()
            .With(x => x.ClienteId, Guid.Empty)
            .Create();

        // Act
        var resultado = await _criarSolicitacaoExclusaoUseCase.Handle(criarSolicitacaoExclusaoDto);

        // Assert
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.BuscarPorClienteId(It.IsAny<Guid>()), Times.Never);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.Criar(It.IsAny<SolicitacaoExclusao>()), Times.Never);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Cliente inválido").Should().Be(1);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoJaHouverSolicitacaoEfetuadaParaOCliente()
    {
        // Arrange
        var criarSolicitacaoExclusaoDto = _fixture.Create<CriarSolicitacaoExclusaoDto>();
        _solicitacaoExclusaoRepositoryMock.Setup(x => 
            x.BuscarPorClienteId(criarSolicitacaoExclusaoDto.ClienteId!.Value)).ReturnsAsync(_fixture.Create<SolicitacaoExclusao>());

        // Act
        var resultado = await _criarSolicitacaoExclusaoUseCase.Handle(criarSolicitacaoExclusaoDto);

        // Assert
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.BuscarPorClienteId(It.IsAny<Guid>()), Times.Once);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.Criar(It.IsAny<SolicitacaoExclusao>()), Times.Never);
        _solicitacaoExclusaoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Solicitação já efetuada!").Should().Be(1);
    }

    /*[Fact]
    public async Task DeveGerarExcecao_QuandoCriarPedidoComProdutoInexistente()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
            .With(x => x.TipoPagamento, TipoPagamento.Pix.ToString())
            .Create();

        _cupomService.Setup(x => x.ObterCupomPorCodigo(It.IsAny<string>())).ReturnsAsync((CupomDto?)null);
        _produtoService.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((ProdutoDto?)null);

        // Act
        Func<Task> act = async () => await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        await act.Should().ThrowAsync<DomainException>().WithMessage("Produto inválido");
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCriarPedidoComTipoPagamentoInvalido()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
            .With(x => x.TipoPagamento, "invalido")
            .Create();

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _estoqueService.Verify(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        _produtoService.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Tipo de pagamento inválido").Should().Be(1);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
            .With(x => x.TipoPagamento, TipoPagamento.Pix.ToString())
            .Create();
        var cupomDto = _fixture.Create<CupomDto>();
        cupomDto.Produtos.Add(new CupomDto.CupomProdutoDto()
        {
            ProdutoId = criarPedidoDto.Itens.First().ProdutoId
        });
        var produtosDto = criarPedidoDto.Itens.Select(x => _fixture.Build<ProdutoDto>()
                                                                .With(k => k.Id, x.ProdutoId)
                                                                .Create()).ToList();

        _cupomService.Setup(x => x.ObterCupomPorCodigo(criarPedidoDto.CodigoCupom!)).ReturnsAsync(cupomDto);
        _estoqueService.Setup(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);
        _pedidoRepository.Setup(x => x.Criar(It.IsAny<Pedido>()));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);
        produtosDto.ForEach(x => _produtoService.Setup(k => k.ObterPorId(x.Id)).ReturnsAsync(x));

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _cupomService.Verify(x => x.ObterCupomPorCodigo(criarPedidoDto.CodigoCupom!), Times.Once);
        _estoqueService.Verify(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(criarPedidoDto.Itens.Count));
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        _produtoService.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Exactly(criarPedidoDto.Itens.Count));
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }*/
}