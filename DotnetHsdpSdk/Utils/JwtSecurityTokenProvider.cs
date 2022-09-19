using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace DotnetHsdpSdk.Utils;

public interface IJwtSecurityTokenProvider
{
    string GenerateJwtToken(string key, string audience, string id);
}

public class JwtSecurityTokenProvider : IJwtSecurityTokenProvider
{
    public string GenerateJwtToken(string key, string audience, string id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var privateKeyBase64 = Regex.Replace(key, "\n", "");
        privateKeyBase64 = Regex.Replace(privateKeyBase64, "-----BEGIN RSA PRIVATE KEY-----", "");
        privateKeyBase64 = Regex.Replace(privateKeyBase64, "-----END RSA PRIVATE KEY-----", "");
        try
        {
            var privateKey = Convert.FromBase64String(privateKeyBase64);
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Issuer = id,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory {CacheSignatureProviders = false}
                }
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Payload.Add("sub", id);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            throw new AuthenticationException("Failed to create service token: " + e);
        }
    }
}
