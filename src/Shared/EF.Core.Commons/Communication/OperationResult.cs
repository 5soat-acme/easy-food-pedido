using System.Collections.ObjectModel;

namespace EF.Core.Commons.Communication;

public class OperationResult
{
    protected OperationResult(List<string>? errors = null)
    {
        ValidationResult = new ValidationResult();

        if (errors is null || !errors.Any())
        {
            IsValid = true;
        }
        else
        {
            IsValid = false;
            foreach (var error in errors) ValidationResult.AddError(error);
        }
    }

    public bool IsValid { get; protected init; }
    public ValidationResult ValidationResult { get; protected init; }

    public ReadOnlyCollection<string> GetErrorMessages()
    {
        return ValidationResult.GetErrorMessages();
    }

    public static OperationResult Success()
    {
        return new OperationResult();
    }

    public static OperationResult Failure(string error)
    {
        return new OperationResult(new List<string> { error });
    }

    public static OperationResult Failure(List<string> errors)
    {
        return new OperationResult(errors);
    }

    public static OperationResult Failure(ReadOnlyCollection<string> errors)
    {
        return new OperationResult(errors.ToList());
    }

    public static OperationResult Failure(ValidationResult validationResult)
    {
        return new OperationResult(validationResult.Errors.Select(e => e.Error).ToList());
    }
}