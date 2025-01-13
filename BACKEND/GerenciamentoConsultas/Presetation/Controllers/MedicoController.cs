using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConsultas.Apresentacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly MedicoAppService _medicoAppService;

        public MedicoController(MedicoAppService medicoAppService)
        {
            _medicoAppService = medicoAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicoById(int id)
        {
            try
            {
                var medico = await _medicoAppService.GetMedicoByIdAsync(id);
                return Ok(medico);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicos()
        {
            var medicos = await _medicoAppService.GetAllMedicosAsync();
            return Ok(medicos);
        }

        [HttpPost]
        public async Task<IActionResult> AddMedico([FromBody] MedicoDTO medicoDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _medicoAppService.AddMedicoAsync(medicoDTO);
            return Ok("Médico adicionado com sucesso.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoDTO medicoDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            try
            {
                medicoDTO.Id = id;

                await _medicoAppService.UpdateMedicoAsync(medicoDTO);
                return Ok("Médico atualizado com sucesso.");
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
        public async Task<IActionResult> DeleteMedico(int id)
        {
            try
            {
                await _medicoAppService.DeleteMedicoAsync(id);
                return Ok("Médico excluído com sucesso.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
