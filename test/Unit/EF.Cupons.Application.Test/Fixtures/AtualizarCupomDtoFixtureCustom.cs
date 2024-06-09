using AutoFixture;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Cupons.Application.Test.Fixtures;

public class AtualizarCupomDtoFixtureCustom : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var startDate = DateTime.Now.AddDays(Math.Abs(fixture.Create<int>()));

        fixture.Customize<AtualizarCupomDto>(composer =>
        composer.With(x => x.DataInicio, startDate)
        .With(x => x.DataFim, startDate.AddDays(Math.Abs(fixture.Create<int>()) + 1))
        .With(x => x.CodigoCupom, fixture.Create<string>().PadRight(4, 'a'))
        .With(x => x.PorcentagemDesconto, GenerateDecimalBetween(0.01, 0.99)));
    }

    private decimal GenerateDecimalBetween(double min, double max)
    {
        Random rand = new Random();
        var valor = Math.Round(rand.NextDouble() * (max - min) + min, 2);
        return (decimal)valor;
    }
}