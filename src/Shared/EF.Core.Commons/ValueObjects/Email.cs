using System.Text.RegularExpressions;
using EF.Core.Commons.DomainObjects;

namespace EF.Core.Commons.ValueObjects;

public class Email
{
    public const int EnderecoMaxLength = 254;

    protected Email()
    {
    }

    public Email(string endereco)
    {
        if (!Validar(endereco)) throw new DomainException("E-mail inválido");
        Endereco = endereco;
    }

    public string Endereco { get; private set; }

    public static bool Validar(string email)
    {
        var regexEmail =
            new Regex(
                @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        return regexEmail.IsMatch(email);
    }
}