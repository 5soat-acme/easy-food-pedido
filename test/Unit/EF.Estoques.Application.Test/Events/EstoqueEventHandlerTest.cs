using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Core.Commons.Messages.Integrations;
using EF.Core.Commons.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Application.Events;
using EF.Estoques.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Estoques.Application.Test.Events;

public class EstoqueEventHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IAtualizarEstoqueUseCase> _atualizarEstoqueUseCaseMock;
    private readonly IEventHandler<PedidoCriadoEvent> _pedidoCriadoEventHandler;
    private readonly IEventHandler<PedidoCanceladoEvent> _pedidoCanceladoEventHandler;

    public EstoqueEventHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _atualizarEstoqueUseCaseMock = _fixture.Freeze<Mock<IAtualizarEstoqueUseCase>>();
        _pedidoCriadoEventHandler = _fixture.Create<EstoqueEventHandler>();
        _pedidoCanceladoEventHandler = _fixture.Create<EstoqueEventHandler>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IAtualizarEstoqueUseCase))).Returns(_atualizarEstoqueUseCaseMock.Object);
    }

    [Fact]
    public async Task DeveAtualizarEstoqueQuandoCriarPedido()
    {
        // Arrange
        var pedidoCriadoEvent = new PedidoCriadoEvent()
        {
            SessionId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Itens = _fixture.CreateMany<PedidoCriadoEvent.ItemPedido>(5).ToList()
        };
        _atualizarEstoqueUseCaseMock.Setup(x => x.Handle(It.IsAny<AtualizarEstoqueDto>())).ReturnsAsync(It.IsAny<OperationResult<Guid>>());

        // Act
        await _pedidoCriadoEventHandler.Handle(pedidoCriadoEvent);

        // Assert
        _atualizarEstoqueUseCaseMock.Verify(x => x.Handle(It.IsAny<AtualizarEstoqueDto>()), Times.Exactly(5));
    }

    [Fact]
    public async Task DeveAtualizarEstoqueQuandoCancelarPedido()
    {
        // Arrange
        var pedidoCanceladoEvent = new PedidoCanceladoEvent()
        {
            Itens = _fixture.CreateMany<PedidoCanceladoEvent.ItemPedido>(5).ToList()
        };
        _atualizarEstoqueUseCaseMock.Setup(x => x.Handle(It.IsAny<AtualizarEstoqueDto>())).ReturnsAsync(It.IsAny<OperationResult<Guid>>());

        // Act
        await _pedidoCanceladoEventHandler.Handle(pedidoCanceladoEvent);

        // Assert
        _atualizarEstoqueUseCaseMock.Verify(x => x.Handle(It.IsAny<AtualizarEstoqueDto>()), Times.Exactly(5));
    }
}