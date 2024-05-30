namespace EF.Core.Commons.Communication;

public class Failure
{
    public Failure(string error, string propertyName = "")
    {
        Error = error;
        PropertyName = propertyName;
    }

    public string PropertyName { get; private set; }
    public string Error { get; private set; }
}