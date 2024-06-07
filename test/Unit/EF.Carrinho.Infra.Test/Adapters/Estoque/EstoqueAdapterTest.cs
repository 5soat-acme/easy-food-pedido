using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Infra.Adapters.Estoque;
using EF.Estoques.Application.UseCases.Interfaces;
using Moq;
using FluentAssertions;

namespace EF.Carrinho.Infra.Test.Adapters.Estoque;

public class EstoqueAdapterTest
{
    private readonly IFixture _fixture;

    public EstoqueAdapterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task DeveVerificarEstoqueDisponivel()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var quantidade = 10;

        var consultaEstoqueUseCaseMock = _fixture.Freeze<Mock<IConsultaEstoqueUseCase>>();
        consultaEstoqueUseCaseMock.Setup(x => x.ValidarEstoque(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(true);

        var adapter = _fixture.Create<EstoqueAdapter>();

        // Act
        var resultado = await adapter.VerificarEstoque(produtoId, quantidade);

        // Assert
        consultaEstoqueUseCaseMock.Verify(x => x.ValidarEstoque(produtoId, quantidade, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task DeveVerificarEstoqueNaoDisponivel()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var quantidade = 10;

        var consultaEstoqueUseCaseMock = _fixture.Freeze<Mock<IConsultaEstoqueUseCase>>();
        consultaEstoqueUseCaseMock.Setup(x => x.ValidarEstoque(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(false);

        var adapter = _fixture.Create<EstoqueAdapter>();

        // Act
        var resultado = await adapter.VerificarEstoque(produtoId, quantidade);

        // Assert
        consultaEstoqueUseCaseMock.Verify(x => x.ValidarEstoque(produtoId, quantidade, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeFalse();
    }
}