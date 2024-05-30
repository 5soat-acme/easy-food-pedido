using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using EF.Infra.Commons.Messageria.AWS.Config;
using Microsoft.Extensions.Options;

namespace EF.Infra.Commons.Messageria.AWS;

public abstract class AwsMessageriaBase
{
    protected readonly IAmazonSQS _sqsClient;

    protected AwsMessageriaBase(IOptions<AwsCredentialsSettings> options)
    {
        var awsCredentialsSettings = options.Value;
        var credentials = new SessionAWSCredentials(awsCredentialsSettings.AccessKey, awsCredentialsSettings.SecretKey, awsCredentialsSettings.SessionToken);
        var region = RegionEndpoint.GetBySystemName(awsCredentialsSettings.Region);
        var sqsConfig = new AmazonSQSConfig
        {
            RegionEndpoint = region
        };
        _sqsClient = new AmazonSQSClient(credentials, sqsConfig);
    }

    protected async Task<string> GetQueueUrlAsync(IAmazonSQS sqsClient, string queueName)
    {
        try
        {
            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = queueName
            };

            var getQueueUrlResponse = await sqsClient.GetQueueUrlAsync(getQueueUrlRequest);

            var queueUrl = getQueueUrlResponse.QueueUrl;
            if (string.IsNullOrWhiteSpace(queueUrl))
            {
                throw new MessageriaException($"Não foi possível encontrar a URL da fila {queueName}");
            }

            return queueUrl;
        }
        catch (Exception ex)
        {
            throw new MessageriaException($"Erro ao obter a URL da fila {queueName}: {ex.Message}");
        }
    }
}
