using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Infra.Commons.Messageria.AWS;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.Events.Consumers;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Infra.Data;
using EF.Produtos.Domain.Models;
using EF.Produtos.Infra.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;

namespace EF.Api.BDD.Test.Support;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public required IServiceProvider serviceProvider;
    private readonly IFixture _fixture;
    private readonly string _dbNamePedido = Guid.NewGuid().ToString();
    private readonly string _dbNameProduto = Guid.NewGuid().ToString();

    public CustomWebApplicationFactory()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(async services => 
        {
            RemoveDbContext(services);
            ConfigureAuth(services);
            RemoveHostedServices(services);
            RemoveProducers(services);
            AddDbContextInMemory(services);
            await CreateDatabases(services);
        });        

        return base.CreateHost(builder);
    }

    private void RemoveDbContext(IServiceCollection services)
    {
        var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<PedidoDbContext>) ||
                     d.ServiceType == typeof(DbContextOptions<ProdutoDbContext>)).ToList();

        foreach(var d in descriptors)
        {
            services.Remove(d);
        }
    }

    private void ConfigureAuth(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Test";
            options.DefaultChallengeScheme = "Test";
        })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    }

    private void RemoveHostedServices(IServiceCollection services)
    {
        var hostedServiceTypes = new[]
        {
            typeof(PagamentoAprovadoConsumer),
            typeof(PagamentoRecusadoConsumer),
            typeof(PreparoPedidoIniciadoConsumer),
            typeof(PreparoPedidoFinalizadoConsumer),
            typeof(EntregaPedidoRealizadaConsumer)
        };

        var descriptors = services.Where(d => hostedServiceTypes.Contains(d.ImplementationType)).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    private void RemoveProducers(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(IProducer) && d.ImplementationType == typeof(AwsProducer)).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        services.AddScoped<IProducer, FakeProducer>();
    }

    private void AddDbContextInMemory(IServiceCollection services)
    {
        services.AddDbContext<PedidoDbContext>(options =>
        {
            options.UseInMemoryDatabase(_dbNamePedido);
        });

        services.AddDbContext<ProdutoDbContext>(options =>
        {
            options.UseInMemoryDatabase(_dbNameProduto);
        });
    }

    private async Task CreateDatabases(IServiceCollection services)
    {
        serviceProvider = services.BuildServiceProvider();
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var dbContextPedido = scopedServices.GetRequiredService<PedidoDbContext>();
            dbContextPedido.Database.EnsureDeleted();
            dbContextPedido.Database.EnsureCreated();

            await SeedPedido(dbContextPedido);

            var dbContextProduto = scopedServices.GetRequiredService<ProdutoDbContext>();
            dbContextProduto.Database.EnsureDeleted();
            dbContextProduto.Database.EnsureCreated();

            await SeedProduto(dbContextProduto);
        }
    }

    private async Task SeedPedido(PedidoDbContext dbContext)
    {
        var pedido = _fixture.Create<Pedido>();
        pedido.Id = Guid.Parse("f694f3a3-2622-45ea-b168-f573f16165ea");

        await dbContext.Pedidos!.AddAsync(pedido);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedProduto(ProdutoDbContext dbContext)
    {
        var produto = new Produto(nome: _fixture.Create<string>(),
            valorUnitario: 10,
            categoria: ProdutoCategoria.Lanche,
            tempoPreparoEstimado: 20,
            descricao: _fixture.Create<string>());
        produto.Id = Guid.Parse("23bf0c51-7230-4a5b-8031-10b3dde9a908");

        await dbContext.Produtos.AddAsync(produto);
        await dbContext.SaveChangesAsync();
    }
}