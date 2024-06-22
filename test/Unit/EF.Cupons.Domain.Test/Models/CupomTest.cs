using EF.Core.Commons.DomainObjects;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Cupons.Domain.Test.Models;

[Collection(nameof(CupomCollection))]
public class CupomTest(CupomFixture fixture)
{
    [Fact]
    public void DeveCriarUmaInstanciaDeCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();

        // Act - Assert
        cupom.Should().BeOfType<Cupom>();
    }

    [Fact]
    public void DeveInativarCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();

        // Act
        cupom.InativarCupom();

        // Assert
        cupom.Status.Should().Be(CupomStatus.Inativo);
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomComDataInicioInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(DateTime.Now.AddDays(-1));

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DataInicio não pode ser inferior a data atual");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomComDataFimInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(DateTime.Now.AddDays(5), DateTime.Now.AddDays(4));

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DataFim não pode ser inferior a DataInicio");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomComCodigoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(codigoCupom: "");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("CodigoCupom inválido");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomComPorcentagemDescontoInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(porcentagemDesconto: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("PorcentagemDesconto inválida");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarCupomComStatusInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(status: (CupomStatus)999);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Status inválido");
    }

    [Fact]
    public void DeveAdicionarProdutoAoCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();
        var cupomProd = fixture.GerarCupomProduto(cupom.Id);

        // Act
        cupom.AdicionarProduto(cupomProd);

        // Assert
        cupom.CupomProdutos.Should().Contain(cupomProd);
    }

    [Fact]
    public void DeveAlterarAlterarDatasDoCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();
        var dataInicio = DateTime.Now;
        var dataFim = DateTime.Now.AddDays(10);

        // Act
        cupom.AlterarDatas(dataInicio, dataFim);

        // Assert
        cupom.DataInicio.Should().Be(dataInicio);
        cupom.DataFim.Should().Be(dataFim);
    }

    [Fact]
    public void DeveAlterarAlterarCodigoCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();
        var codigoCupom = "cupom-teste";

        // Act
        cupom.AlterarCodigoCupom(codigoCupom);

        // Assert
        cupom.CodigoCupom.Should().Be(codigoCupom);
    }

    [Fact]
    public void DeveAlterarPorcentagemDesconto()
    {
        // Arrange
        var cupom = fixture.GerarCupom();
        var porcentagemDesconto = 0.5M;

        // Act
        cupom.AlterarPorcentagemDesconto(porcentagemDesconto);

        // Assert
        cupom.PorcentagemDesconto.Should().Be(porcentagemDesconto);
    }
}