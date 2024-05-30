using EF.Core.Commons.DomainObjects;

namespace EF.Cupons.Domain.Models;

public class Cupom : Entity, IAggregateRoot
{
    private readonly List<CupomProduto> _cupomProdutos;

    private Cupom()
    {
        _cupomProdutos = new List<CupomProduto>();
    }

    public Cupom(DateTime dataInicio, DateTime dataFim, string codigoCupom, decimal porcentagemDesconto,
        CupomStatus status)
    {
        ValidarCupom(dataInicio, dataFim, codigoCupom, porcentagemDesconto, status);

        _cupomProdutos = new List<CupomProduto>();

        DataInicio = dataInicio;
        DataFim = dataFim;
        CodigoCupom = codigoCupom;
        PorcentagemDesconto = porcentagemDesconto;
        Status = status;
    }

    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public string CodigoCupom { get; private set; }
    public decimal PorcentagemDesconto { get; private set; }
    public CupomStatus Status { get; private set; }
    public IReadOnlyCollection<CupomProduto> CupomProdutos => _cupomProdutos;

    private void ValidarCupom(DateTime dataInicio, DateTime dataFim, string codigoCupom, decimal porcentagemDesconto,
        CupomStatus status)
    {
        ValidarDatas(dataInicio, dataFim);
        ValidarCodigoCupom(codigoCupom);
        ValidarPorcentagemDesconto(porcentagemDesconto);
        ValidarCupomStatus(status);
    }

    public void AdicionarProduto(CupomProduto cupomProduto)
    {
        var itemExistente = _cupomProdutos.Count(f => f.ProdutoId == cupomProduto.ProdutoId) > 0;
        if (!itemExistente) _cupomProdutos.Add(cupomProduto);
    }

    public void InativarCupom()
    {
        Status = CupomStatus.Inativo;
    }

    public void AlterarDatas(DateTime dataInicio, DateTime dataFim)
    {
        ValidarDatas(dataInicio, dataFim);
        DataInicio = dataInicio;
        DataFim = dataFim;
    }

    public void AlterarCodigoCupom(string codigoCupom)
    {
        ValidarCodigoCupom(codigoCupom);
        CodigoCupom = codigoCupom;
    }

    public void AlterarPorcentagemDesconto(decimal porcentagemDesconto)
    {
        ValidarPorcentagemDesconto(porcentagemDesconto);
        PorcentagemDesconto = porcentagemDesconto;
    }

    private void ValidarDatas(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio < DateTime.Now.Date) throw new DomainException("DataInicio não pode ser inferior a data atual");
        if (dataFim < dataInicio) throw new DomainException("DataFim não pode ser inferior a DataInicio");
    }

    private void ValidarCodigoCupom(string codigoCupom)
    {
        if (string.IsNullOrEmpty(codigoCupom) || codigoCupom.Length < 3)
            throw new DomainException("CodigoCupom inválido");
    }

    private void ValidarPorcentagemDesconto(decimal porcentagemDesconto)
    {
        if (porcentagemDesconto <= 0 || porcentagemDesconto > 0.99M)
            throw new DomainException("PorcentagemDesconto inválida");
    }

    private void ValidarCupomStatus(CupomStatus status)
    {
        if (!Enum.IsDefined(typeof(CupomStatus), status)) throw new DomainException("Status inválido");
    }
}