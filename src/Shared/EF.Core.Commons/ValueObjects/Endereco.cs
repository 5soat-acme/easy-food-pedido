using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.Utils;

namespace EF.Core.Commons.ValueObjects;

public class Endereco
{
    protected Endereco()
    {
    }

    public Endereco(string rua, int numero, string bairro, string complemento, string cidade, string estado, string cep)
    {        
        if (!Validar(rua, numero, bairro, complemento, cidade, estado, cep)) throw new DomainException("Endereço inválido");
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Complemento = complemento;
        Cidade = cidade;
        Estado = estado;
        Cep = cep.SomenteNumeros(cep);
    }

    public string Rua { get; private set; }
    public int Numero { get; private set; }
    public string Bairro { get; private set; }
    public string Complemento { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public string Cep { get; private set; }

    public bool Validar(string rua, int numero, string bairro, string complemento, string cidade, string estado, string cep)
    {
        if (string.IsNullOrWhiteSpace(rua) || 
            numero < 0 || 
            string.IsNullOrWhiteSpace(bairro) ||
            string.IsNullOrWhiteSpace(cidade) ||
            string.IsNullOrWhiteSpace(estado) ||
            string.IsNullOrWhiteSpace(cep))
        {
            return false;
        }
        return true;
    }
}