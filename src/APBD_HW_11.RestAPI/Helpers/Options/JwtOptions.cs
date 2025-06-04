namespace APBD_HW_11.RestAPI.Helpers.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
}
