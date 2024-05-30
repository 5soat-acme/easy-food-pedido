namespace EF.Infra.Commons.Extensions;

public static class DateTimeExtension
{
    public static long ToUnixEpochDate(this DateTime date)
    {
        return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
    }
}