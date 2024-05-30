namespace EF.Infra.Commons.Messageria;
public interface IProducer
{
    Task SendMessageAsync(string queueName, string messageBody);
}
