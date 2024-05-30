using Amazon.SQS.Model;

namespace EF.Infra.Commons.Messageria.AWS.Models;

public class AWSConsumerResponse
{
    public ReceiveMessageResponse receiveMessageResponse { get; set; }
    public string queueUrl { get; set; }
}
