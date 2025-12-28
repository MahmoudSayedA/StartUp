using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Identity.JWT;
public class JwtDecoder
{
    public void DecodeJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Decode header
        var header = jwtToken.Header;
        Console.WriteLine("Header:");
        foreach (var item in header)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }

        // Decode payload
        var payload = jwtToken.Payload;
        Console.WriteLine("\nPayload:");
        foreach (var item in payload)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }
}
