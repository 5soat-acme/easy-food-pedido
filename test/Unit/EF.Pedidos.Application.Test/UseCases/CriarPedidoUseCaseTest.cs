using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using EF.Pedidos.Application.Gateways;
using EF.Pedidos.Domain.Repository;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Application.UseCases;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Pedidos.Domain.Models;
using FluentAssertions;
using EF.Core.Commons.DomainObjects;

namespace EF.Pedidos.Application.Test.UseCases;

public class CriarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICupomService> _cupomService;
    private readonly Mock<IEstoqueService> _estoqueService;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly Mock<IProdutoService> _produtoService;
    private readonly ICriarPedidoUseCase _criarPedidoUseCase;

    public CriarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _cupomService = _fixture.Freeze<Mock<ICupomService>>();
        _estoqueService = _fixture.Freeze<Mock<IEstoqueService>>();
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _produtoService = _fixture.Freeze<Mock<IProdutoService>>();
        _criarPedidoUseCase = _fixture.Create<CriarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveCriarPedidoComCupom()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
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
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);
        produtosDto.ForEach(x => _produtoService.Setup(k => k.ObterPorId(x.Id)).ReturnsAsync(x));

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _cupomService.Verify(x => x.ObterCupomPorCodigo(criarPedidoDto.CodigoCupom!), Times.Once);
        _estoqueService.Verify(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(criarPedidoDto.Itens.Count));
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        _produtoService.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Exactly(criarPedidoDto.Itens.Count));
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCriarPedidoComProdutoQuePossuiEstoqueInsuficiente()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
            .Create();
        var produtosDto = criarPedidoDto.Itens.Select(x => _fixture.Build<ProdutoDto>()
                                                                .With(k => k.Id, x.ProdutoId)
                                                                .Create()).ToList();

        _cupomService.Setup(x => x.ObterCupomPorCodigo(It.IsAny<string>())).ReturnsAsync((CupomDto?)null);
        _estoqueService.Setup(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);
        produtosDto.ForEach(x => _produtoService.Setup(k => k.ObterPorId(x.Id)).ReturnsAsync(x));

        // Act
        var resultado = await _criarPedidoUseCase.Handle(criarPedidoDto);

        // Assert
        _estoqueService.Verify(x => x.VerificarEstoque(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(criarPedidoDto.Itens.Count));
        _pedidoRepository.Verify(x => x.Criar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        _produtoService.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Exactly(criarPedidoDto.Itens.Count));
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Estoque insuficiente").Should().Be(criarPedidoDto.Itens.Count);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCriarPedidoComProdutoInexistente()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
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
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var criarPedidoDto = _fixture.Build<CriarPedidoDto>()
            .With(x => x.ClienteCpf, "90190274093")
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
    }
}