namespace EF.Infra.Commons.Messageria;

public class MessageriaException : Exception
{
    public MessageriaException()
    {
    }

    public MessageriaException(string message) : base(message)
    {
    }

    public MessageriaException(string message, Exception innerException) : base(message, innerException)
    {
    }
}