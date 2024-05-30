using EF.Infra.Commons.Messageria;
using EF.Infra.Commons.Messageria.AWS;
using EF.Infra.Commons.Messageria.AWS.Config;
using EF.Infra.Commons.Messageria.AWS.Models;

namespace EF.Api.Commons.Config;

public static class MessageriaConfig
{
    public static IServiceCollection AddMessageriaConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsCredentialsSettings>(configuration.GetSection("AWSCredentials"));

        services.AddScoped<IProducer, AwsProducer>();
        services.AddSingleton<IConsumer<AWSConsumerResponse, AwsConfirmReceipt>, AwsConsumer>();

        return services;
    }
}
