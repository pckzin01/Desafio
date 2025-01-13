using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConsultas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioAppService _usuarioAppService;
        private readonly JWTService _jwtService;

        public AuthController(UsuarioAppService usuarioAppService, JWTService jwtService)
        {
            _usuarioAppService = usuarioAppService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                // Validar credenciais
                var usuario = await _usuarioAppService.AuthenticateAsync(loginDTO.Email, loginDTO.Senha);
                if (usuario == null)
                    return Unauthorized(new { Message = "Credenciais inválidas." });

                // Gerar token JWT
                var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.TipoUsuario.ToString());

                return Ok(new
                {
                    Token = token,
                    Message = "Login realizado com sucesso.",
                    Expiration = DateTime.UtcNow.AddHours(24)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar login: {ex.Message}");
            }
        }
    }
}
