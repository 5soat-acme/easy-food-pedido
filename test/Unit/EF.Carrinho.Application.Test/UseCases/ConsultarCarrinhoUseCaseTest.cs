using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.DTOs.Responses;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Carrinho.Application.Test.UseCases;

public class ConsultarCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly IConsultarCarrinhoUseCase _consultarCarrinhoUseCase;

    public ConsultarCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _consultarCarrinhoUseCase = _fixture.Create<ConsultarCarrinhoUseCase>();
    }

    [Fact]
    public async Task DeveConsultarCarrrinhoPorClienteId()
    {
        // Arrange
        var carrinhoSessao = new CarrinhoSessaoDto()
        {
            CarrinhoId = _fixture.Create<Guid>(),
            ClienteId = _fixture.Create<Guid>()
        };

        var item = _fixture.Create<Item>();
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCliente(carrinhoSessao.ClienteId.Value);
        carrinho.AssociarCarrinho(carrinhoSessao.CarrinhoId);
        carrinho.AdicionarItem(item);
        var carrinhoDto = new CarrinhoClienteDto()
        {
            Id = carrinho.Id,
            ClienteId = carrinho.ClienteId,
            ValorTotal = carrinho.ValorTotal,
            TempoMedioPreparo = carrinho.TempoMedioPreparo,
            Itens = carrinho.Itens.Select(x => new ItemCarrinhoDto()
            {
                Id = x.Id,
                ProdutoId = x.ProdutoId,
                Nome = x.Nome,
                Quantidade = x.Quantidade,
                ValorUnitario = x.ValorUnitario,
                TempoEstimadoPreparo = x.TempoEstimadoPreparo                
            })            
        };


        _carrinhoRepositoryMock.Setup(x => x.ObterPorClienteId(carrinhoSessao.ClienteId.Value)).ReturnsAsync(carrinho);

        // Act
        var resultado = await _consultarCarrinhoUseCase.ConsultarCarrinho(carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.ObterPorClienteId(carrinhoSessao.ClienteId.Value), Times.Once);
        resultado.Should().BeEquivalentTo(carrinhoDto);
    }

    [Fact]
    public async Task DeveConsultarCarrrinhoPorCarrinhoId()
    {
        // Arrange
        var carrinhoSessao = new CarrinhoSessaoDto()
        {
            CarrinhoId = _fixture.Create<Guid>()
        };

        var item = _fixture.Create<Item>();
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(carrinhoSessao.CarrinhoId);
        carrinho.AdicionarItem(item);
        var carrinhoDto = new CarrinhoClienteDto()
        {
            Id = carrinho.Id,
            ClienteId = carrinho.ClienteId,
            ValorTotal = carrinho.ValorTotal,
            TempoMedioPreparo = carrinho.TempoMedioPreparo,
            Itens = carrinho.Itens.Select(x => new ItemCarrinhoDto()
            {
                Id = x.Id,
                ProdutoId = x.ProdutoId,
                Nome = x.Nome,
                Quantidade = x.Quantidade,
                ValorUnitario = x.ValorUnitario,
                TempoEstimadoPreparo = x.TempoEstimadoPreparo
            })
        };


        _carrinhoRepositoryMock.Setup(x => x.ObterPorId(carrinhoSessao.CarrinhoId)).ReturnsAsync(carrinho);

        // Act
        var resultado = await _consultarCarrinhoUseCase.ConsultarCarrinho(carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.ObterPorId(carrinhoSessao.CarrinhoId), Times.Once);
        resultado.Should().BeEquivalentTo(carrinhoDto);
    }

    [Fact]
    public async Task DeveConsultarCarrrinhoInexistentePorClienteIdECriarCarrinhoNovo()
    {
        // Arrange
        var carrinhoSessao = new CarrinhoSessaoDto()
        {
            CarrinhoId = _fixture.Create<Guid>(),
            ClienteId = _fixture.Create<Guid>()
        };

        _carrinhoRepositoryMock.Setup(x => x.ObterPorClienteId(carrinhoSessao.ClienteId.Value)).ReturnsAsync((CarrinhoCliente?)null);

        // Act
        var resultado = await _consultarCarrinhoUseCase.ConsultarCarrinho(carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.ObterPorClienteId(carrinhoSessao.ClienteId.Value), Times.Once);
        resultado.ClienteId.Should().Be(carrinhoSessao.ClienteId);
    }
}