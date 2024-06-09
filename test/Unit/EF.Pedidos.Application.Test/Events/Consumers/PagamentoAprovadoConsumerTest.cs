using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.Events.Consumers;
using EF.Pedidos.Application.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Amazon.SQS.Model;
using EF.Core.Commons.Communication;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Events.Queues;
using System.Text.Json;
using EF.Pedidos.Application.DTOs.Requests;

namespace EF.Pedidos.Application.Test.Events.Consumers;

public class PagamentoAprovadoConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>> _consumerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IReceberPedidoUsecase> _receberPedidoUsecase;
    private readonly PagamentoAprovadoConsumer _pagamentoAprovadoConsumer;

    public PagamentoAprovadoConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consumerMock = _fixture.Freeze<Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>>>();
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _receberPedidoUsecase = _fixture.Freeze<Mock<IReceberPedidoUsecase>>();
        _pagamentoAprovadoConsumer = _fixture.Create<PagamentoAprovadoConsumer>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IReceberPedidoUsecase))).Returns(_receberPedidoUsecase.Object);
    }

    [Fact]
    public async Task DeveExecutarConsumerCompleto()
    {
        // Arrange
        var pagamentoAutorizadoEvent = _fixture.Create<PagamentoAutorizadoEvent>();
        var message = new Message
        {
            Body = JsonSerializer.Serialize(pagamentoAutorizadoEvent),
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

        _consumerMock.Setup(x => x.ReceiveMessagesAsync(QueuesNames.PagamentoAutorizado.ToString())).ReturnsAsync(response);
        _receberPedidoUsecase.Setup(x => x.Handle(It.IsAny<ReceberPedidoDto>())).ReturnsAsync(It.IsAny<OperationResult<Guid>>());

        // Act
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            cancellationTokenSource.CancelAfter(300);
            await _pagamentoAprovadoConsumer.StartAsync(cancellationTokenSource.Token);
        }

        // Assert
        _consumerMock.Verify(c => c.ReceiveMessagesAsync(It.IsAny<string>()));
        _consumerMock.Verify(c => c.ConfirmReceiptAsync(It.Is<AwsConfirmReceipt>(confirm =>
            confirm.QueueUrl == response.queueUrl &&
            confirm.ReceiptHandle == message.ReceiptHandle
        )));
        _receberPedidoUsecase.Verify(u => u.Handle(It.Is<ReceberPedidoDto>(dto =>
            dto.PedidoId == pagamentoAutorizadoEvent.PedidoId
        )));
    }
}
