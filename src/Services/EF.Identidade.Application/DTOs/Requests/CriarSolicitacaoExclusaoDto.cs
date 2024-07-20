using EF.Core.Commons.ValueObjects;
using System.Text.Json.Serialization;

namespace EF.Identidade.Application.DTOs.Requests;

public class CriarSolicitacaoExclusaoDto
{
    [JsonIgnore]
    public Guid? ClienteId { get; set; }
    public string Nome { get; set; }
    public string Rua { get; set; }
    public int Numero { get; set; }
    public string Bairro { get; set; }
    public string Complemento { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public string NumeroTelefone { get; set; }
}