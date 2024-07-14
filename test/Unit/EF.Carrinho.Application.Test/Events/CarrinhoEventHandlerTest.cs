using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Application.UseCases.Interfaces;
using Moq;
using EF.Core.Commons.Messages.Integrations;
using EF.Core.Commons.Messages;
using EF.Carrinho.Application.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Carrinho.Application.Test.Events;

public class CarrinhoEventHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider>  _serviceProviderMock;
    private readonly Mock<IRemoverCarrinhoUseCase>  _removerCarrinhoUseCaseMock;
    private readonly IEventHandler<PedidoCriadoEvent> _carrinhoEventHandler;

    public CarrinhoEventHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _removerCarrinhoUseCaseMock = _fixture.Freeze<Mock<IRemoverCarrinhoUseCase>>();
        _carrinhoEventHandler = _fixture.Create<CarrinhoEventHandler>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IRemoverCarrinhoUseCase))).Returns(_removerCarrinhoUseCaseMock.Object);
    }

    [Fact]
    public async Task DeveRemoverCarrinhoPorClienteId()
    {
        // Arrange
        var pedidoCriadoEvent = new PedidoCriadoEvent()
        {
            SessionId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid()
        };
        _removerCarrinhoUseCaseMock.Setup(x => x.RemoverCarrinhoPorClienteId(pedidoCriadoEvent.ClienteId.Value)).Returns(Task.CompletedTask);

        // Act
        await _carrinhoEventHandler.Handle(pedidoCriadoEvent);

        // Assert
        _removerCarrinhoUseCaseMock.Verify(x => x.RemoverCarrinhoPorClienteId(pedidoCriadoEvent.ClienteId!.Value), Times.Once);
        _removerCarrinhoUseCaseMock.Verify(x => x.RemoverCarrinho(pedidoCriadoEvent.SessionId), Times.Never);
    }

    [Fact]
    public async Task DeveRemoverCarrinhoPorSessionId()
    {
        // Arrange
        var pedidoCriadoEvent = new PedidoCriadoEvent()
        {
            SessionId = Guid.NewGuid()
        };
        _removerCarrinhoUseCaseMock.Setup(x => x.RemoverCarrinho(pedidoCriadoEvent.SessionId)).Returns(Task.CompletedTask);

        // Act
        await _carrinhoEventHandler.Handle(pedidoCriadoEvent);

        // Assert        
        _removerCarrinhoUseCaseMock.Verify(x => x.RemoverCarrinho(pedidoCriadoEvent.SessionId), Times.Once);
        _removerCarrinhoUseCaseMock.Verify(x => x.RemoverCarrinhoPorClienteId(It.IsAny<Guid>()), Times.Never);
    }
}
