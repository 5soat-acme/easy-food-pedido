namespace EF.Test.Utils
{
    public static class UtilsTest
    {
        public static T GetRandomEnum<T>(Array values) where T : Enum
        {
            Random random = new();
            var enumRetorno = (T)values.GetValue(random.Next(values.Length))!;

            return enumRetorno;
        }
    }
}
