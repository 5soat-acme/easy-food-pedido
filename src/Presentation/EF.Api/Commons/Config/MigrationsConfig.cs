using EF.Carrinho.Infra.Data;
using EF.Cupons.Infra;
using EF.Estoques.Infra;
using EF.Pedidos.Infra.Data;
using EF.Produtos.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Commons.Config;

public static class MigrationsConfig
{
    public static WebApplication RunMigrations(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();

            var carrinho = scope.ServiceProvider.GetRequiredService<CarrinhoDbContext>();
            carrinho.Database.Migrate();

            var cupons = scope.ServiceProvider.GetRequiredService<CupomDbContext>();
            cupons.Database.Migrate();

            var estoque = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
            estoque.Database.Migrate();

            var pedidos = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
            pedidos.Database.Migrate();

            var produtos = scope.ServiceProvider.GetRequiredService<ProdutoDbContext>();
            produtos.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return app;
    }
}