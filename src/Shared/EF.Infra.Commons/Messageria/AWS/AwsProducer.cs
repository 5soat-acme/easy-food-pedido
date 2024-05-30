using Amazon.SQS.Model;
using EF.Infra.Commons.Messageria.AWS.Config;
using Microsoft.Extensions.Options;

namespace EF.Infra.Commons.Messageria.AWS;

public class AwsProducer : AwsMessageriaBase, IProducer
{

    public AwsProducer(IOptions<AwsCredentialsSettings> options) : base(options)
    {
    }

    public async Task SendMessageAsync(string queueName, string messageBody)
    {
        try
        {
            var queueUrl = await GetQueueUrlAsync(_sqsClient, queueName);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            var sendMessageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
            throw new MessageriaException($"Erro ao enviar a mensagem para a fila {queueName}: {ex.Message}");
        }
    }
}
