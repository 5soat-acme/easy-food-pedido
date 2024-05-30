using System.Collections.ObjectModel;

namespace EF.Core.Commons.Communication;

public class OperationResult<T> : OperationResult
{
    private OperationResult(List<string>? errors = null, T? data = default)
        : base(errors)
    {
        Data = data;
    }

    public T? Data { get; private set; }

    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>(data: data);
    }

    public static OperationResult<T> Failure(string error)
    {
        return new OperationResult<T>(new List<string> { error });
    }

    public static OperationResult<T> Failure(List<string> errors)
    {
        return new OperationResult<T>(errors);
    }

    public static OperationResult<T> Failure(ReadOnlyCollection<string> errors)
    {
        return new OperationResult<T>(errors.ToList());
    }

    public static OperationResult<T> Failure(ValidationResult validationResult)
    {
        return new OperationResult<T>(validationResult.Errors.Select(e => e.Error).ToList());
    }
}