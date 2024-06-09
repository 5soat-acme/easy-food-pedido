using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Domain.Repository;
using Moq;

namespace EF.Carrinho.Application.Test.UseCases;

public class RemoverItemCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly Mock<IConsultarCarrinhoUseCase> _consultarCarrinhoUseCase;
    private readonly IRemoverItemCarrinhoUseCase _removerItemCarrinhoUseCase;

    public RemoverItemCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _consultarCarrinhoUseCase = _fixture.Freeze<Mock<IConsultarCarrinhoUseCase>>();
        _removerItemCarrinhoUseCase = _fixture.Create<RemoverItemCarrinhoUseCase>();
    }


}
