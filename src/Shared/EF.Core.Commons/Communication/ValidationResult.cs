using System.Collections.ObjectModel;

namespace EF.Core.Commons.Communication;

public class ValidationResult
{
    private readonly List<Failure> _errors = new();
    public ReadOnlyCollection<Failure> Errors => _errors.AsReadOnly();
    public bool IsValid => !_errors.Any();

    public void AddError(string message, string propertyName = "")
    {
        _errors.Add(new Failure(message, propertyName));
    }

    public ReadOnlyCollection<string> GetErrorMessages()
    {
        var messages = _errors.Select(e => e.Error).ToList();
        return messages.AsReadOnly();
    }
}