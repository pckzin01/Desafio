using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConsultas.Apresentacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioAppService _usuarioAppService;

        public UsuarioController(UsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }


        [HttpPost]
        public async Task<IActionResult> AddUsuario([FromBody] UsuarioInserirDTO usuarioInserirDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Chama o serviço para criar o usuário e retorna o ID do usuário criado
                var usuarioId = await _usuarioAppService.AddUsuarioAsync(usuarioInserirDTO);

                // Retorna o ID do usuário criado junto com a mensagem de sucesso
                return CreatedAtAction(nameof(GetUsuarioById), new { id = usuarioId }, new
                {
                    message = "Usuário criado com sucesso!",
                    id = usuarioId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            try
            {
                var usuario = await _usuarioAppService.GetUsuarioByIdAsync(id);
                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            try
            {
                var usuarios = await _usuarioAppService.GetAllUsuariosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                usuarioDTO.Id = id;

                await _usuarioAppService.UpdateUsuarioAsync(usuarioDTO);
                return Ok("Usuário atualizado com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }




       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                await _usuarioAppService.DeleteUsuarioAsync(id);
                return Ok("Usuário excluído com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }
    }
}
