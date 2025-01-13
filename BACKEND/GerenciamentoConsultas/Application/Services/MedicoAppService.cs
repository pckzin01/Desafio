using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Validators;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;

namespace GerenciamentoConsultas.Application.Services
{
    public class MedicoAppService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public MedicoAppService(IMedicoRepository medicoRepository, IUsuarioRepository usuarioRepository)
        {
            _medicoRepository = medicoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<MedicoDTO> GetMedicoByIdAsync(int id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico == null)
                throw new KeyNotFoundException("Médico não encontrado.");

            return new MedicoDTO
            {
                Id = medico.Id,
                Especialidade = medico.Especialidade,
                CRM = medico.CRM,
                UsuarioId = medico.UsuarioId
            };
        }

        public async Task<IEnumerable<MedicoDTO>> GetAllMedicosAsync()
        {
            var medicos = await _medicoRepository.GetAllAsync();
            return medicos.Select(m => new MedicoDTO
            {
                Id = m.Id,
                Especialidade = m.Especialidade,
                CRM = m.CRM,
                UsuarioId = m.UsuarioId
            });
        }

        public async Task AddMedicoAsync(MedicoDTO medicoDTO)
        {
            // Verificar se o CRM é válido
            if (!CrmValidator.IsValidCRM(medicoDTO.CRM))
                throw new ArgumentException("CRM inválido. Verifique o formato e a sigla do estado.");

            // Verificar se o UsuarioId é válido
            var usuarioExists = await _usuarioRepository.ExistsAsync(medicoDTO.UsuarioId);
            if (!usuarioExists)
                throw new KeyNotFoundException("Usuário vinculado não encontrado.");

            var medico = new Medico
            {
                Especialidade = medicoDTO.Especialidade,
                CRM = medicoDTO.CRM,
                UsuarioId = medicoDTO.UsuarioId
            };

            await _medicoRepository.AddAsync(medico);

            medicoDTO.Id = medico.Id;
        }


        public async Task UpdateMedicoAsync(MedicoDTO medicoDTO)
        {
            var medico = await _medicoRepository.GetByIdAsync(medicoDTO.Id);
            if (medico == null)
                throw new KeyNotFoundException("Médico não encontrado.");

            // Verificar se o UsuarioId é válido
            var usuarioExists = await _usuarioRepository.ExistsAsync(medicoDTO.UsuarioId);
            if (!usuarioExists)
                throw new KeyNotFoundException("Usuário vinculado não encontrado.");

            medico.Especialidade = medicoDTO.Especialidade;
            medico.CRM = medicoDTO.CRM;
            medico.UsuarioId = medicoDTO.UsuarioId;

            await _medicoRepository.UpdateAsync(medico);
        }

        public async Task DeleteMedicoAsync(int id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico == null)
                throw new KeyNotFoundException("Médico não encontrado.");

            await _medicoRepository.DeleteAsync(id);
        }
    }
}
