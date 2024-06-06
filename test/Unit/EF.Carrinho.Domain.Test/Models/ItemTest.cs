using EF.Carrinho.Domain.Models;
using EF.Carrinho.Domain.Test.Fixtures;
using EF.Core.Commons.DomainObjects;
using FluentAssertions;

namespace EF.Carrinho.Domain.Test.Models
{

    [Collection(nameof(CarrinhoCollection))]
    public class ItemTest(CarrinhoClienteFixture fixture)
    {
        [Fact(DisplayName = "Criar item válido")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveCriarItemValido()
        {
            // Arrange
            var item = fixture.GerarItem();

            // Act & Assert 
            item.Should().BeOfType<Item>();
            item.Id.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "Criar item com produto inválido")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveGerarExcecao_QuandoCriarItemComProdutoInvalido()
        {
            // Arrange
            Action act = () => fixture.GerarItem(produtoId: Guid.Empty);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Produto inválido");
        }

        [Theory(DisplayName = "Criar item com valor unitário inválido")]
        [Trait("Category", "Domain - Carrinho")]
        [InlineData(-1)]
        [InlineData(0)]
        public void DeveGerarExcecao_QuandoCriarItemComValorUnitarioInvalido(decimal valorUnitario)
        {
            // Arrange & Act
            Action act = () => fixture.GerarItem(valorUnitario: valorUnitario);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Valor unitário inválido");
        }

        [Fact(DisplayName = "Associar carrinho")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveAssociarCarrinho()
        {
            // Arrange
            var item = fixture.GerarItem();
            var carrinhoId = Guid.NewGuid();

            // Act
            item.AssociarCarrinho(carrinhoId);

            // Assert
            item.CarrinhoId.Should().NotBeEmpty();
            item.CarrinhoId.Should().Be(carrinhoId);
        }

        [Fact(DisplayName = "Associar carrinho inválido")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveGerarExcecao_QuandoAssociarCarrinhoInvalido()
        {
            // Arrange
            var item = fixture.GerarItem();

            // Act
            Action act = () => item.AssociarCarrinho(Guid.Empty);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Id do carrinho inválido");
        }

        [Fact(DisplayName = "Atualizar quantidade")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveAtualizarQuantidade()
        {
            // Arrange
            var item = fixture.GerarItem();

            // Act
            int qtd = 10;
            item.AtualizarQuantidade(qtd);

            // Assert
            item.Quantidade.Should().Be(qtd);
        }

        [Fact(DisplayName = "Atualizar quantidade inválida")]
        [Trait("Category", "Domain - Carrinho")]
        public void DeveGerarExcecao_QuandoAtualizarQuantidadeInvalida()
        {
            // Arrange
            var item = fixture.GerarItem();

            // Act
            int qtd = -1;
            Action act = () => item.AtualizarQuantidade(qtd);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Quantidade inválida");
        }
    }
}
