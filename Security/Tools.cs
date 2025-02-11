namespace CityCard_API.Security;
static internal class Tools{
    static internal string ComputeHash(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashed = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashed);
    }
    static internal string GenerateToken(){
        var rawToken = Guid.NewGuid().ToString();
        return rawToken;
    }
}