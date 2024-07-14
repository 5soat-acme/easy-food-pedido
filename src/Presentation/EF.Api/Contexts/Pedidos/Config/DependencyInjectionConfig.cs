using EF.Pedidos.Application.Gateways;
using EF.Pedidos.Application.UseCases;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Repository;
using EF.Pedidos.Infra.Adapters.Cupons;
using EF.Pedidos.Infra.Adapters.Estoque;
using EF.Pedidos.Infra.Adapters.Produtos;
using EF.Pedidos.Infra.Data;
using EF.Pedidos.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Pedidos.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServicesPedidos(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application - UseCases
        services.AddScoped<IAtualizarPedidoUseCase, AtualizarPedidoUseCase>();
        services.AddScoped<IConsultarPedidoUseCase, ConsultarPedidoUseCase>();
        services.AddScoped<ICriarPedidoUseCase, CriarPedidoUseCase>();
        services.AddScoped<IReceberPedidoUsecase, ReceberPedidoUsecase>();
        services.AddScoped<ICancelarPedidoUseCase, CancelarPedidoUseCase>();

        // Application - Gateways & Gateways
        services.AddScoped<IEstoqueService, EstoqueAdapter>();
        services.AddScoped<ICupomService, CupomAdapter>();
        services.AddScoped<IProdutoService, ProdutoAdapter>();

        // Infra - Data
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddDbContext<PedidoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}