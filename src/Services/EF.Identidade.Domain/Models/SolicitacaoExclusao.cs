using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.Utils;
using EF.Core.Commons.ValueObjects;

namespace EF.Identidade.Domain.Models;

public class SolicitacaoExclusao : Entity, IAggregateRoot
{
    protected SolicitacaoExclusao() { }

    public SolicitacaoExclusao(Guid clienteId, string nome, Endereco endereco, string numeroTelefone)
    {
        ValidarSolicitacao(clienteId, nome, endereco, numeroTelefone);

        ClienteId = clienteId;
        Nome = nome;
        Endereco = endereco;
        NumeroTelefone = numeroTelefone.SomenteNumeros(numeroTelefone);
    }

    public Guid ClienteId { get; private set; }
    public string Nome { get; private set; }
    public Endereco Endereco { get; private set; }
    public string NumeroTelefone { get; private set; }

    public void ValidarSolicitacao(Guid clienteId, string nome, Endereco endereco, string numeroTelefone)
    {
        ValidarCliente(clienteId);
        ValidarNome(nome);
        ValidarEndereco(endereco);
        ValidarNumeroTelefone(numeroTelefone);
    }

    public void ValidarCliente(Guid clienteId)
    {
        if (clienteId == Guid.Empty) throw new DomainException("Id do cliente inválido");
    }

    public void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome inválido");
    }

    public void ValidarEndereco(Endereco endereco)
    {
        if (endereco == null) throw new DomainException("Endereço inválido");
    }

    public void ValidarNumeroTelefone(string numeroTelefone)
    {
        if (string.IsNullOrWhiteSpace(numeroTelefone)) throw new DomainException("Número de telefone inválido");
    }
}
