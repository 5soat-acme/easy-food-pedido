using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using EF.Pedidos.Application.Events.Queues;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Application.DTOs.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Pedidos.Application.Events.Consumers;

public class PagamentoAprovadoConsumer : BackgroundService
{
    private readonly IConsumer<AWSConsumerResponse, AwsConfirmReceipt> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PagamentoAprovadoConsumer(IConsumer<AWSConsumerResponse, AwsConfirmReceipt> consumer,
                        IServiceScopeFactory serviceScopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            var receberPedidoUsecase = scope.ServiceProvider.GetRequiredService<IReceberPedidoUsecase>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _consumer.ReceiveMessagesAsync(QueuesNames.PagamentoAutorizado.ToString());

                    foreach (var message in response.receiveMessageResponse.Messages)
                    {
                        var pagamentoAutorizado = JsonSerializer.Deserialize<PagamentoAutorizadoEvent>(message.Body);

                        if (pagamentoAutorizado != null)
                        {
                            await receberPedidoUsecase.Handle(new ReceberPedidoDto
                            {
                                PedidoId = pagamentoAutorizado.PedidoId
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
                catch (Exception ex)
                {
                    // Log de erros ou manipulação de exceções
                }
            }
        }
    }
}
