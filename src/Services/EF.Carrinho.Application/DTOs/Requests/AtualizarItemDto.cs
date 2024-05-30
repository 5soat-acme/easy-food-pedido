using System.ComponentModel.DataAnnotations;
using EF.WebApi.Commons.ModelStateValidations;

namespace EF.Carrinho.Application.DTOs.Requests;

public class AtualizarItemDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [NonEmptyGuid(ErrorMessage = "O campo {0} não é válido")]
    public Guid ItemId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser maior que 0")]
    public int Quantidade { get; set; }
}