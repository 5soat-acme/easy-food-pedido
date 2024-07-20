using AutoFixture;
using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.ValueObjects;
using EF.Identidade.Domain.Models;
using FluentAssertions;

namespace EF.Identidade.Domain.Test.Models
{
    public class SolicitacaoExclusaoTest
    {
        [Fact]
        public void DeveCriarSolicitacaoExclusao()
        {
            // Arrange
            var solicitacao = new SolicitacaoExclusao(Guid.NewGuid(), "Cliente 1", 
                    new Endereco("Rua 1", 1, "Bairro 1", "", "Cidade 1", "SP", "11111-111") , "(11)11111-1111");

            // Act & Assert 
            solicitacao.Should().NotBeNull();
        }

        [Fact]
        public void DeveGerarExcecao_QuandoCriarSolicitacaoComClienteInvalido()
        {
            // Arrange
            Action act = () => new SolicitacaoExclusao(Guid.Empty, "Cliente 1",
                    new Endereco("Rua 1", 1, "Bairro 1", "", "Cidade 1", "SP", "11111-111"), "(11)11111-1111");
            // Arrange & Act & Assert 
            act.Should().Throw<DomainException>().WithMessage("Id do cliente inválido");
        }

        [Fact]
        public void DeveGerarExcecao_QuandoCriarSolicitacaoComNomeInvalido()
        {
            // Arrange
            Action act = () => new SolicitacaoExclusao(Guid.NewGuid(), "",
                    new Endereco("Rua 1", 1, "Bairro 1", "", "Cidade 1", "SP", "11111-111"), "(11)11111-1111");
            // Arrange & Act & Assert 
            act.Should().Throw<DomainException>().WithMessage("Nome inválido");
        }

        [Fact]
        public void DeveGerarExcecao_QuandoCriarSolicitacaoComEnderecoNuloInvalido()
        {
            // Arrange
            Action act = () => new SolicitacaoExclusao(Guid.NewGuid(), "Cliente 1", null, "(11)11111-1111");
            // Arrange & Act & Assert 
            act.Should().Throw<DomainException>().WithMessage("Endereço inválido");
        }

        [Fact]
        public void DeveGerarExcecao_QuandoCriarSolicitacaoComEnderecoInvalido()
        {
            // Arrange
            Action act = () => new SolicitacaoExclusao(Guid.NewGuid(), "Cliente 1",
                    new Endereco("", 1, "Bairro 1", "", "Cidade 1", "SP", "11111-111"), "(11)11111-1111");
            // Arrange & Act & Assert 
            act.Should().Throw<DomainException>().WithMessage("Endereço inválido");
        }

        [Fact]
        public void DeveGerarExcecao_QuandoCriarSolicitacaoComNumeroTelefoneInvalido()
        {
            // Arrange
            Action act = () => new SolicitacaoExclusao(Guid.NewGuid(), "Cliente 1",
                    new Endereco("Rua 1", 1, "Bairro 1", "", "Cidade 1", "SP", "11111-111"), "");
            // Arrange & Act & Assert 
            act.Should().Throw<DomainException>().WithMessage("Número de telefone inválido");
        }
    }
}
