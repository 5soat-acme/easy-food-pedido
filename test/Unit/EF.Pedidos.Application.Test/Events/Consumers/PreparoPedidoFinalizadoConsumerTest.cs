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
using EF.Pedidos.Application.Events.Queues;
using System.Text.Json;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Domain.Models;

namespace EF.Pedidos.Application.Test.Events.Consumers;

public class PreparoPedidoFinalizadoConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>> _consumerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IAtualizarPedidoUseCase> _atualizarPedidoUseCaseMock;
    private readonly PreparoPedidoFinalizadoConsumer _preparoPedidoFinalizadoConsumer;

    public PreparoPedidoFinalizadoConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _consumerMock = _fixture.Freeze<Mock<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>>>();
        _serviceScopeFactoryMock = _fixture.Freeze<Mock<IServiceScopeFactory>>();
        _serviceScopeMock = _fixture.Freeze<Mock<IServiceScope>>();
        _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
        _atualizarPedidoUseCaseMock = _fixture.Freeze<Mock<IAtualizarPedidoUseCase>>();
        _preparoPedidoFinalizadoConsumer = _fixture.Create<PreparoPedidoFinalizadoConsumer>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IAtualizarPedidoUseCase))).Returns(_atualizarPedidoUseCaseMock.Object);
    }

    [Fact]
    public async Task DeveExecutarConsumerCompleto()
    {
        // Arrange
        var entregaRealizadaEvent = _fixture.Create<EntregaRealizadaEvent>();
        var message = new Message
        {
            Body = JsonSerializer.Serialize(entregaRealizadaEvent),
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

        _consumerMock.Setup(x => x.ReceiveMessagesAsync(QueuesNames.PreparoPedidoFinalizado.ToString())).ReturnsAsync(response);
        _atualizarPedidoUseCaseMock.Setup(x => x.Handle(It.IsAny<AtualizarPedidoDto>())).ReturnsAsync(It.IsAny<OperationResult>);

        // Act
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            cancellationTokenSource.CancelAfter(300);
            await _preparoPedidoFinalizadoConsumer.StartAsync(cancellationTokenSource.Token);
        }

        // Assert
        _consumerMock.Verify(c => c.ReceiveMessagesAsync(It.IsAny<string>()));
        _consumerMock.Verify(c => c.ConfirmReceiptAsync(It.Is<AwsConfirmReceipt>(confirm =>
            confirm.QueueUrl == response.queueUrl &&
            confirm.ReceiptHandle == message.ReceiptHandle
        )));
        _atualizarPedidoUseCaseMock.Verify(u => u.Handle(It.Is<AtualizarPedidoDto>(dto =>
            dto.PedidoId == entregaRealizadaEvent.PedidoCorrelacaoId &&
            dto.Status == Status.Pronto
        )));
    }
}
