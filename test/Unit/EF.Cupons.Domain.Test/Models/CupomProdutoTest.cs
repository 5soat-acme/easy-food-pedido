﻿using EF.Core.Commons.DomainObjects;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Cupons.Domain.Test.Models;

[Collection(nameof(CupomCollection))]
public class CupomProdutoTest(CupomFixture fixture)
{
    [Fact]
    public void DeveCriarUmaInstanciaDeCupomProduto()
    {
        // Arrange
        var movimentacao = fixture.GerarCupomProduto(Guid.NewGuid());

        // Act - Assert
        movimentacao.Should().BeOfType<CupomProduto>();
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomProdutoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupomProduto(Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Um CupomProduto deve estar associado a um Cupom");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomProdutoComProdutoIdInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupomProduto(Guid.NewGuid(), Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Produto inválido");
    }
}