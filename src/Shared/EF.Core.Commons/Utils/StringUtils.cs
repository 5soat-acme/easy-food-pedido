namespace EF.Core.Commons.Utils;

public static class StringUtils
{
    public static string SomenteNumeros(this string str, string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }
}