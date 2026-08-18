using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration config) : ControllerBase
{
    /// <summary>Autentica e retorna um JWT. Credenciais de teste no README.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> Login(LoginRequest request)
    {
        if (request.Usuario != "admin" || request.Senha != "Fast@2026")
            return Unauthorized(new { title = "Credenciais inválidas", status = 401 });

        var jwt = config.GetSection("Jwt");
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: [new Claim(ClaimTypes.Name, request.Usuario)],
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiraEmMinutos"]!)),
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}

public record LoginRequest([Required] string Usuario, [Required] string Senha);
