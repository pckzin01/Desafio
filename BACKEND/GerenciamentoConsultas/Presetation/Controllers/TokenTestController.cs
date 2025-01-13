using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConsultas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenTestController : ControllerBase
    {
        // Endpoint protegido por autenticação
        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetProtectedData()
        {
            return Ok(new
            {
                Message = "O token é válido!",
                User = User.Identity?.Name, // Retorna o nome do usuário autenticado, se houver
                Claims = User.Claims.Select(c => new { c.Type, c.Value }) // Retorna os claims do token
            });
        }

        // Endpoint público (sem autenticação)
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult GetPublicData()
        {
            return Ok(new
            {
                Message = "Este é um endpoint público que não requer autenticação."
            });
        }
    }
}
