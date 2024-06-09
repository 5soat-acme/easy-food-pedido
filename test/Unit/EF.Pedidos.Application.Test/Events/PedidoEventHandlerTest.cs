using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Infra.Commons.Messageria;
using Moq;
using EF.Pedidos.Application.Events;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Events.Queues;
using System.Text.Json;

namespace EF.Pedidos.Application.Test.Events;

public class PedidoEventHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IProducer> _producerMock;
    private readonly PedidoEventHandler _pedidoEventHandler;

    public PedidoEventHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _producerMock = _fixture.Freeze<Mock<IProducer>>();
        _pedidoEventHandler = _fixture.Create<PedidoEventHandler>();
    }

    [Fact]
    public async Task DeveExecutarEventoPedidoRecebido()
    {
        // Arrange
        var pedidoRecebidoEvent = _fixture.Create<PedidoRecebidoEvent>();
        var pedidoRecebidoEventJson = JsonSerializer.Serialize(pedidoRecebidoEvent);

        _producerMock.Setup(x => x.SendMessageAsync(QueuesNames.PedidoRecebido.ToString(), pedidoRecebidoEventJson));

        // Act
        await _pedidoEventHandler.Handle(pedidoRecebidoEvent);

        // Assert
        _producerMock.Verify(x => x.SendMessageAsync(QueuesNames.PedidoRecebido.ToString(), pedidoRecebidoEventJson), Times.Once);
    }
}