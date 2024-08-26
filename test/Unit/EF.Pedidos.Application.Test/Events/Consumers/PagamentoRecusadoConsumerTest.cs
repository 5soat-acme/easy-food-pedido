using Amazon.SQS.Model;
using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Core.Commons.Communication;
using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.Events.Consumers;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using EF.Pedidos.Application.UseCases;
using System.Text.Json;
using EF.Pedidos.Application.Events.Queues;
using EF.Pedidos.Application.DTOs.Requests;

namespace EF.Pedidos.Application.Test.Events.Consumers;

public class PagamentoRecusadoConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>> _consumerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ICancelarPedidoUseCase> _cancelarPedidoUseCase;
    private readonly PagamentoRecusadoConsumer _pagamentoRecusadoConsumer;

    public PagamentoRecusadoConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consumerMock = _fixture.Freeze<Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>>>();
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _cancelarPedidoUseCase = _fixture.Freeze<Mock<ICancelarPedidoUseCase>>();
        _pagamentoRecusadoConsumer = _fixture.Create<PagamentoRecusadoConsumer>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ICancelarPedidoUseCase))).Returns(_cancelarPedidoUseCase.Object);
    }

    [Fact]
    public async Task DeveExecutarConsumerCompleto()
    {
        // Arrange
        var pagamentoRecusadoEvent = _fixture.Create<PagamentoRecusadoEvent>();
        var message = new Message
        {
            Body = JsonSerializer.Serialize(pagamentoRecusadoEvent),
            ReceiptHandle = "receipt-handle"
        };

        var response = new AWSConsumerResponse
        {
            receiveMessageResponse = new ReceiveMessageResponse
            {
                Messages = new List<Message> { message }
            },
            queueUrl = "queue-url"
        };

        _consumerMock.Setup(x => x.ReceiveMessagesAsync(QueuesNames.PagamentoRecusado.ToString())).ReturnsAsync(response);
        _cancelarPedidoUseCase.Setup(x => x.Handle(It.IsAny<CancelarPedidoDto>())).ReturnsAsync(It.IsAny<OperationResult<Guid>>());

        // Act
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            cancellationTokenSource.CancelAfter(300);
            await _pagamentoRecusadoConsumer.StartAsync(cancellationTokenSource.Token);
        }

        // Assert
        _consumerMock.Verify(c => c.ReceiveMessagesAsync(It.IsAny<string>()));
        _consumerMock.Verify(c => c.ConfirmReceiptAsync(It.Is<AwsConfirmReceipt>(confirm =>
            confirm.QueueUrl == response.queueUrl &&
            confirm.ReceiptHandle == message.ReceiptHandle
        )));
        _cancelarPedidoUseCase.Verify(u => u.Handle(It.Is<CancelarPedidoDto>(dto =>
            dto.PedidoId == pagamentoRecusadoEvent.PedidoId
        )));
    }
}
