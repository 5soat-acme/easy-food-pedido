using System.ComponentModel.DataAnnotations;

namespace EF.WebApi.Commons.ModelStateValidations;

public class NonEmptyGuidAttribute : ValidationAttribute
{
    public NonEmptyGuidAttribute()
    {
        ErrorMessage = "O GUID não pode ser vazio.";
    }

    public override bool IsValid(object value)
    {
        if (value == null) return true;
        if (!(value is Guid)) return false;

        var guidValue = (Guid)value;
        return guidValue != Guid.Empty;
    }
}