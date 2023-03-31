using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DemoLogin.Database;
using DemoLogin.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DemoLogin.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAuthController : ControllerBase
{
    private readonly TestEntities _entities;
    private readonly IConfiguration _configuration;

    public UserAuthController(TestEntities entities, IConfiguration configuration) : base()
    {
        _entities = entities;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        model.Username = model.Username.ToLower().Trim();
        model.Password = model.Password.Trim();
        var acc = _entities.Users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
        if (acc == null)
            return new JsonResult(new { error = true, message = "Wrong email or password, plese check again !" });

        var token = GenerateToken(acc);
        return Ok(new { Token = token });
    }

    private string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
            expires: DateTime.Now.AddDays(1), signingCredentials: credential);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}




