using EF.Infra.Commons.Messageria.AWS.Models;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Events.Queues;
using EF.Pedidos.Application.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace EF.Pedidos.Application.Events.Consumers;

public class PagamentoRecusadoConsumer : BackgroundService
{
    private readonly IConsumer<AWSConsumerResponse, AwsConfirmReceipt> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PagamentoRecusadoConsumer(IConsumer<AWSConsumerResponse, AwsConfirmReceipt> consumer,
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
                var response = await _consumer.ReceiveMessagesAsync(QueuesNames.PagamentoRecusado.ToString());

                foreach (var message in response.receiveMessageResponse.Messages)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        var cancelarPedidoUseCase = scope.ServiceProvider.GetRequiredService<ICancelarPedidoUseCase>();
                        var pagamentoRecusado = JsonSerializer.Deserialize<PagamentoRecusadoEvent>(message.Body);

                        if (pagamentoRecusado != null)
                        {
                            await cancelarPedidoUseCase.Handle(new CancelarPedidoDto
                            {
                                PedidoId = pagamentoRecusado.PedidoId
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
            }
            catch (Exception ex) { }
        }
    }
}
