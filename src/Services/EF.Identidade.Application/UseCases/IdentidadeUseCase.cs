using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EF.Core.Commons.Communication;
using EF.Identidade.Application.DTOs.Responses;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.Infra.Commons.Extensions;
using EF.WebApi.Commons.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;
using UsuarioClaim = EF.Identidade.Application.DTOs.Responses.UsuarioClaim;

namespace EF.Identidade.Application.UseCases;

public class IdentidadeUseCase : IIdentidadeUseCase
{
    private readonly IdentitySettings _identitySettings;

    public IdentidadeUseCase(IOptions<IdentitySettings> settings) {
        _identitySettings = settings.Value;
    }

    public async Task<OperationResult<RespostaTokenAcesso>> AcessarSistema()
    {
        return AcessarComUsuarioNaoRegistrado();
    }

    private OperationResult<RespostaTokenAcesso> AcessarComUsuarioNaoRegistrado(string? cpf = null)
    {
        var result = GerarTokenUsuarioNaoIdentificado(cpf);
        return OperationResult<RespostaTokenAcesso>.Success(result);
    }

    private RespostaTokenAcesso GerarTokenUsuarioNaoIdentificado(string? cpf = null)
    {
        var tokenClaims = ObterTokenClaims();

        tokenClaims.AddClaim(new Claim("user_type", "anonymous"));
        tokenClaims.AddClaim(new Claim("session_id", Guid.NewGuid().ToString()));

        if (!string.IsNullOrEmpty(cpf)) tokenClaims.AddClaim(new Claim("user_cpf", cpf));

        var encodedToken = CodificarToken(tokenClaims);
        return ObterRespostaToken(encodedToken, tokenClaims.Claims);
    }

    private ClaimsIdentity ObterTokenClaims()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Nbf,
                DateTime.UtcNow.ToUnixEpochDate().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTime.UtcNow.ToUnixEpochDate().ToString())
        };

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        return identityClaims;
    }    

    private string CodificarToken(ClaimsIdentity identityClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _identitySettings.Issuer,
            Audience = _identitySettings.ValidIn,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(_identitySettings.ExpirationHours),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }

    private RespostaTokenAcesso ObterRespostaToken(string encodedToken,
        IEnumerable<Claim> claims)
    {
        return new RespostaTokenAcesso
        {
            Token = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_identitySettings.ExpirationHours).TotalSeconds,
            User = new UsuarioToken
            {
                Id = null,
                Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
            }
        };
    }
}