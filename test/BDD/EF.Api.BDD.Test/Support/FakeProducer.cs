using EF.Infra.Commons.Messageria;

namespace EF.Api.BDD.Test.Support;

public class FakeProducer : IProducer
{
    public Task SendMessageAsync(string queueName, string messageBody)
    {
        return Task.CompletedTask;
    }
}