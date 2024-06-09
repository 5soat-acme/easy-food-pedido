using AutoFixture;
using EF.Cupons.Domain.Models;

namespace EF.Cupons.Application.Test.Fixtures;

public class CupomFixtureCustom : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var startDate = DateTime.Now.AddDays(Math.Abs(fixture.Create<int>()));

        fixture.Customize<Cupom>(composer =>
        composer.FromFactory(() => new Cupom(
                                    dataInicio: startDate,
                                    dataFim: startDate.AddDays(Math.Abs(fixture.Create<int>()) + 1),
                                    codigoCupom: fixture.Create<string>().PadRight(4, 'a'),
                                    porcentagemDesconto: GenerateDecimalBetween(0.01, 0.99),
                                    status: fixture.Create<CupomStatus>())));
    }

    private decimal GenerateDecimalBetween(double min, double max)
    {
        Random rand = new Random();
        var valor = Math.Round(rand.NextDouble() * (max - min) + min, 2);
        return (decimal)valor;
    }
}