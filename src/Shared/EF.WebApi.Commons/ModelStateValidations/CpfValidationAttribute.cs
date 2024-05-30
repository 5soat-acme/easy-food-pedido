using System.ComponentModel.DataAnnotations;
using EF.Core.Commons.ValueObjects;

namespace EF.WebApi.Commons.ModelStateValidations;

public class CpfValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string cpf) throw new ArgumentException("CPF deve ser uma string");

        if (!Cpf.Validar(cpf)) return new ValidationResult("CPF inválido");

        return ValidationResult.Success!;
    }
}