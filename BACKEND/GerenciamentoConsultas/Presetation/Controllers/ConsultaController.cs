using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoConsultas.Apresentacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        private readonly ConsultaAppService _consultaAppService;

        public ConsultaController(ConsultaAppService consultaAppService)
        {
            _consultaAppService = consultaAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var consulta = await _consultaAppService.GetConsultaByIdAsync(id);
                return Ok(consulta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await _consultaAppService.GetAllConsultasAsync();
            return Ok(consultas);
        }

        [HttpPost]
        public async Task<IActionResult> CriarConsulta([FromBody] ConsultaDTO consultaDTO)
        {
            try
            {
                await _consultaAppService.AddConsultaAsync(consultaDTO);
                return Ok("Consulta criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar consulta: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsultaDTO consultaDTO)
        {
            // Validar se o ID no corpo da requisição corresponde ao ID da URL
            if (consultaDTO.Id != 0 && consultaDTO.Id != id)
                return BadRequest("O ID da consulta na URL não corresponde ao ID no corpo da requisição.");

            consultaDTO.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _consultaAppService.UpdateConsultaAsync(consultaDTO);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int pacienteId)
        {
            try
            {
                await _consultaAppService.DeleteConsultaAsync(id, pacienteId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
