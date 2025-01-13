namespace GerenciamentoConsultas.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using GerenciamentoConsultas.Application.Services;
    using GerenciamentoConsultas.Application.DTOs;

    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteAppService _pacienteAppService;

        public PacienteController(PacienteAppService pacienteAppService)
        {
            _pacienteAppService = pacienteAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pacientes = await _pacienteAppService.GetAllAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var paciente = await _pacienteAppService.GetByIdAsync(id);
                return Ok(paciente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PacienteDTO pacienteDTO)
        {
            try
            {
                await _pacienteAppService.AddAsync(pacienteDTO);
                return CreatedAtAction(nameof(GetById), new { id = pacienteDTO.Id }, pacienteDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao adicionar paciente: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteDTO pacienteDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            try
            {
                pacienteDTO.Id = id;

                await _pacienteAppService.UpdateAsync(pacienteDTO);
                return Ok("Paciente atualizado com sucesso.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar paciente: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _pacienteAppService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar paciente: {ex.Message}");
            }
        }
    }
}
