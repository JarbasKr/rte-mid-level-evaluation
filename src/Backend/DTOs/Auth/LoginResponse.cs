namespace Rte.Api.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Bearer";
    public DateTime Expiration { get; set; }
    public DateTime ExpiraEm { get; set; }
    public string Login { get; set; } = string.Empty;
    public AuthUserResponse User { get; set; } = new();
}
