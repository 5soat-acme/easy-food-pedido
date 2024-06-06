using EF.Core.Commons.DomainObjects;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Cupons.Domain.Test.Models;

[Collection(nameof(CupomCollection))]
public class CupomTest(CupomFixture fixture)
{
    [Fact(DisplayName = "Novo cupom válido")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveCriarUmaInstanciaDeCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();

        // Act - Assert
        cupom.Should().BeOfType<Cupom>();
    }

    [Fact(DisplayName = "Inativar cupom")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveInativarCupom()
    {
        // Arrange
        var cupom = fixture.GerarCupom();

        // Act
        cupom.InativarCupom();

        // Assert
        cupom.Status.Should().Be(CupomStatus.Inativo);
    }

    [Fact(DisplayName = "Novo cupom com data início inválida")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveGerarExcecao_QuandoCriarCupomComDataInicioInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(DateTime.Now.AddDays(-1));

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DataInicio não pode ser inferior a data atual");
    }

    [Fact(DisplayName = "Novo cupom com data fim inválida")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveGerarExcecao_QuandoCriarCupomComDataFimInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(DateTime.Now.AddDays(5), DateTime.Now.AddDays(4));

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DataFim não pode ser inferior a DataInicio");
    }

    [Fact(DisplayName = "Novo cupom com código inválido")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveGerarExcecao_QuandoCriarCupomComCodigoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(codigoCupom: "");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("CodigoCupom inválido");
    }

    [Fact(DisplayName = "Novo cupom com porcentagem de desconto inválida")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveGerarExcecao_QuandoCriarCupomComPorcentagemDescontoInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(porcentagemDesconto: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("PorcentagemDesconto inválida");
    }

    [Fact(DisplayName = "Novo cupom com status inválido")]
    [Trait("Category", "Domain - Cupom")]
    public void DeveGerarExcecao_QuandoCriarCupomComStatusInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarCupom(status: (CupomStatus)999);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Status inválido");
    }

    [Fact(DisplayName = "Adicionar produto ao cupom")]
    [Trait("Category", "Domain - Cupom")]
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

    [Fact(DisplayName = "Alterar datas")]
    [Trait("Category", "Domain - Cupom")]
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

    [Fact(DisplayName = "Alterar código do cupom")]
    [Trait("Category", "Domain - Cupom")]
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

    [Fact(DisplayName = "Alterar porcentagem desconto do cupom")]
    [Trait("Category", "Domain - Cupom")]
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