// ConsultaAppService.cs
using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;

namespace GerenciamentoConsultas.Application.Services
{
    public class ConsultaAppService
    {
        private readonly IConsultaRepository _consultaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMedicoRepository _medicoRepository;

        public ConsultaAppService(
            IConsultaRepository consultaRepository,
            IUsuarioRepository usuarioRepository,
            IMedicoRepository medicoRepository)
        {
            _consultaRepository = consultaRepository;
            _usuarioRepository = usuarioRepository;
            _medicoRepository = medicoRepository;
        }

        public async Task<ConsultaDTO> GetConsultaByIdAsync(int id)
        {
            var consulta = await _consultaRepository.GetByIdAsync(id);
            if (consulta == null)
                throw new KeyNotFoundException("Consulta não encontrada.");

            return new ConsultaDTO
            {
                Id = consulta.Id,
                MedicoId = consulta.MedicoId,
                PacienteId = consulta.PacienteId,
                DataHora = consulta.DataHora,
                Status = (DTOs.StatusConsulta)consulta.Status,
                Observacoes = consulta.Observacoes
            };
        }

        public async Task<IEnumerable<ConsultaDTO>> GetAllConsultasAsync()
        {
            var consultas = await _consultaRepository.GetAllAsync();
            return consultas.Select(c => new ConsultaDTO
            {
                Id = c.Id,
                MedicoId = c.MedicoId,
                PacienteId = c.PacienteId,
                DataHora = c.DataHora,
                Status = (DTOs.StatusConsulta)c.Status,
                Observacoes = c.Observacoes
            });
        }

        public async Task AddConsultaAsync(ConsultaDTO consultaDTO)
        {
            // Validar se o paciente existe
            var paciente = await _usuarioRepository.GetByIdAsync(consultaDTO.PacienteId);
            if (paciente == null)
                throw new Exception("Paciente não encontrado.");

            // Validar se o médico existe
            var medico = await _medicoRepository.GetByIdAsync(consultaDTO.MedicoId);
            if (medico == null)
                throw new Exception("Médico não encontrado.");

            // Criar a consulta
            var consulta = new Consulta
            {
                PacienteId = consultaDTO.PacienteId,
                MedicoId = consultaDTO.MedicoId,
                DataHora = consultaDTO.DataHora,
                Status = (Domain.Entities.StatusConsulta)consultaDTO.Status,
                Observacoes = consultaDTO.Observacoes
            };

            await _consultaRepository.AddAsync(consulta);
        }


        public async Task UpdateConsultaAsync(ConsultaDTO consultaDTO)
        {
            // Carregar a consulta
            var consulta = await _consultaRepository.GetByIdAsync(consultaDTO.Id);
            if (consulta == null)
                throw new KeyNotFoundException("Consulta não encontrada.");

            // Validar se o paciente existe
            var usuario = await _usuarioRepository.GetByIdAsync(consultaDTO.PacienteId);
            if (usuario == null)
                throw new Exception("Paciente não encontrado.");

            // Validar se o paciente é o proprietário da consulta
            if (usuario.TipoUsuario == TipoUsuario.Paciente && consulta.PacienteId != consultaDTO.PacienteId)
                throw new Exception("Você não tem permissão para atualizar esta consulta.");

            // Atualizar os valores
            consulta.MedicoId = consultaDTO.MedicoId;
            consulta.DataHora = consultaDTO.DataHora;
            consulta.Status = (Domain.Entities.StatusConsulta)consultaDTO.Status;
            consulta.Observacoes = consultaDTO.Observacoes;

            await _consultaRepository.UpdateAsync(consulta);
        }

        public async Task DeleteConsultaAsync(int id, int pacienteId)
        {
            var consulta = await _consultaRepository.GetByIdAsync(id);
            if (consulta == null)
                throw new KeyNotFoundException("Consulta não encontrada.");

            // Validar se o paciente é o proprietário da consulta
            if (consulta.PacienteId != pacienteId)
                throw new Exception("Você não tem permissão para excluir esta consulta.");

            await _consultaRepository.DeleteAsync(id);
        }
    }
}
