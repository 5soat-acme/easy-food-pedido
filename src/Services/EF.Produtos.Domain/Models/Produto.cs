using EF.Core.Commons.DomainObjects;

namespace EF.Produtos.Domain.Models;

public enum ProdutoCategoria
{
    Lanche = 0,
    Acompanhamento = 1,
    Bebida = 2,
    Sobremesa = 3
}

public class Produto : Entity, IAggregateRoot
{
    public Produto(string nome, decimal valorUnitario, ProdutoCategoria categoria, int tempoPreparoEstimado,
        string descricao)
    {
        ValidarProduto(nome, valorUnitario, categoria, descricao, tempoPreparoEstimado);

        Nome = nome;
        ValorUnitario = valorUnitario;
        Ativo = true;
        Categoria = categoria;
        TempoPreparoEstimado = tempoPreparoEstimado;
        Descricao = descricao;
    }

    public string Nome { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public bool Ativo { get; private set; }
    public ProdutoCategoria Categoria { get; private set; }
    public int TempoPreparoEstimado { get; private set; }
    public string Descricao { get; private set; }

    public void ValidarProduto(string nome, decimal valorUnitario, ProdutoCategoria categoria, string descricao,
        int tempPreparoEstimado)
    {
        ValidarNome(nome);
        ValidarValorUnitario(valorUnitario);
        ValidarCategoria(categoria);
        ValidarDescricao(descricao);
        ValidarTempoPreparoEstimado(tempPreparoEstimado);
    }

    public void AlterarProduto(string nome, decimal valorUnitario, ProdutoCategoria categoria, string descricao,
        int tempoPreparoEstimado, bool ativo)
    {
        AlterarNome(nome);
        AlterarValorUnitario(valorUnitario);
        AlterarCategoria(categoria);
        AlterarDescricao(descricao);
        AlterarTempoPreparoEstimado(tempoPreparoEstimado);

        if (ativo)
            Ativar();
        else
            Desativar();
    }

    public void AlterarNome(string nome)
    {
        ValidarNome(nome);
        Nome = nome;
    }

    public void ValidarNome(string nome)
    {
        if (string.IsNullOrEmpty(nome)) throw new DomainException("Nome inválido");
    }

    public void AlterarValorUnitario(decimal valorUnitario)
    {
        ValidarValorUnitario(valorUnitario);
        ValorUnitario = valorUnitario;
    }

    public void ValidarValorUnitario(decimal valorUnitario)
    {
        if (valorUnitario <= 0) throw new DomainException("Valor unitário inválido");
    }

    public void AlterarCategoria(ProdutoCategoria categoria)
    {
        ValidarCategoria(categoria);
        Categoria = categoria;
    }

    public void AlterarDescricao(string descricao)
    {
        ValidarDescricao(descricao);
        Descricao = descricao;
    }

    public void AlterarTempoPreparoEstimado(int tempoPreparoEstimado)
    {
        ValidarTempoPreparoEstimado(tempoPreparoEstimado);
        TempoPreparoEstimado = tempoPreparoEstimado;
    }

    public void ValidarCategoria(ProdutoCategoria categoria)
    {
        if (!Enum.IsDefined(typeof(ProdutoCategoria), categoria)) throw new DomainException("Categoria inválida");
    }

    public void ValidarDescricao(string descricao)
    {
        if (string.IsNullOrEmpty(descricao)) throw new DomainException("Descrição inválida");
    }

    public void ValidarTempoPreparoEstimado(int tempoPreparoEstimado)
    {
        if (tempoPreparoEstimado <= 0) throw new DomainException("TempoPreparoEstimado inválido");
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}