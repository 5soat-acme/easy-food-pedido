using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.Events.Queues;
using EF.Pedidos.Application.UseCases.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using EF.Pedidos.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using EF.Pedidos.Application.Events.Messages;

namespace EF.Pedidos.Application.Events.Consumers;

public class PreparoPedidoFinalizadoConsumer : BackgroundService
{
    private readonly IConsumer<AWSConsumerResponse, AwsConfirmReceipt> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PreparoPedidoFinalizadoConsumer(IConsumer<AWSConsumerResponse, AwsConfirmReceipt> consumer,
                        IServiceScopeFactory serviceScopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _consumer.ReceiveMessagesAsync(QueuesNames.PreparoPedidoFinalizado.ToString());

                foreach (var message in response.receiveMessageResponse.Messages)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        var atualizarPedidoUseCase = scope.ServiceProvider.GetRequiredService<IAtualizarPedidoUseCase>();
                        var pedidoFinalizado = JsonSerializer.Deserialize<PreparoPedidoFinalizadoEvent>(message.Body);

                        if (pedidoFinalizado != null)
                        {
                            await atualizarPedidoUseCase.Handle(new AtualizarPedidoDto
                            {
                                PedidoId = pedidoFinalizado.PedidoCorrelacaoId,
                                Status = Status.Pronto
                            });

                            var confirm = new AwsConfirmReceipt
                            {
                                QueueUrl = response.queueUrl,
                                ReceiptHandle = message.ReceiptHandle
                            };

                            await _consumer.ConfirmReceiptAsync(confirm);
                        }
                    }
                }
            } catch (Exception ex) { }
        }
    }
}