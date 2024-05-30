using Amazon.SQS.Model;
using EF.Infra.Commons.Messageria.AWS.Config;
using EF.Infra.Commons.Messageria.AWS.Models;
using Microsoft.Extensions.Options;

namespace EF.Infra.Commons.Messageria.AWS;

public class AwsConsumer : AwsMessageriaBase, IConsumer<AWSConsumerResponse, AwsConfirmReceipt>
{
    public AwsConsumer(IOptions<AwsCredentialsSettings> options) : base(options)
    {

    }

    public async Task<AWSConsumerResponse> ReceiveMessagesAsync(string queueName)
    {
        try
        {
            var queueUrl = await GetQueueUrlAsync(_sqsClient, queueName);

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 5
            };

            var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
            return new AWSConsumerResponse
            {
                receiveMessageResponse = receiveMessageResponse,
                queueUrl = queueUrl
            };
        }
        catch (Exception ex)
        {
            throw new MessageriaException($"Erro ao receber a mensagem da fila {queueName}: {ex.Message}");
        }
    }

    public async Task ConfirmReceiptAsync(AwsConfirmReceipt obj)
    {
        try
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = obj.QueueUrl,
                ReceiptHandle = obj.ReceiptHandle
            };

            await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
        }
        catch (Exception ex)
        {
            throw new MessageriaException($"Erro ao confirmar o recebimento da mensagem da fila {obj.QueueUrl}: {ex.Message}");
        }
    }
}
