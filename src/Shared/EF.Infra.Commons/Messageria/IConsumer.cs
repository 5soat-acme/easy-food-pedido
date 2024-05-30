namespace EF.Infra.Commons.Messageria;

public interface IConsumer<T, K>
{
    Task<T> ReceiveMessagesAsync(string queueName);
    Task ConfirmReceiptAsync(K obj);
}