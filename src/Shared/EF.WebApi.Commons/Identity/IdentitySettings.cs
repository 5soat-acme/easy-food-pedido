namespace EF.WebApi.Commons.Identity;

public class IdentitySettings
{
    public string Secret { get; set; }
    public int ExpirationHours { get; set; }
    public string Issuer { get; set; }
    public string ValidIn { get; set; }
}