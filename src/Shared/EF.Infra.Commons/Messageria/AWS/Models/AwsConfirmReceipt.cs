namespace EF.Infra.Commons.Messageria.AWS.Models;

public class AwsConfirmReceipt
{
    public string QueueUrl { get; set; }
    public string ReceiptHandle { get; set; }
}
