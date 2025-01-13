namespace GerenciamentoConsultas.Application.Services
{
    using GerenciamentoConsultas.Application.DTOs;
    using GerenciamentoConsultas.Domain.Entities;
    using GerenciamentoConsultas.Domain.Interfaces;

    public class PacienteAppService
    {
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteAppService(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        public async Task<PacienteDTO?> GetByIdAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);

            return paciente == null ? null : new PacienteDTO
            {
                Id = paciente.Id,
                DataNascimento = paciente.DataNascimento,
                Telefone = paciente.Telefone,
                Endereco = paciente.Endereco,
                UsuarioId = paciente.UsuarioId
            };
        }

        public async Task<IEnumerable<PacienteDTO>> GetAllAsync()
        {
            var pacientes = await _pacienteRepository.GetAllAsync();

            return pacientes.Select(p => new PacienteDTO
            {
                Id = p.Id,
                DataNascimento = p.DataNascimento,
                Telefone = p.Telefone,
                Endereco = p.Endereco,
                UsuarioId = p.UsuarioId
            });
        }

        public async Task AddAsync(PacienteDTO pacienteDTO)
        {
            var paciente = new Paciente
            {
                DataNascimento = pacienteDTO.DataNascimento,
                Telefone = pacienteDTO.Telefone,
                Endereco = pacienteDTO.Endereco,
                UsuarioId = pacienteDTO.UsuarioId
            };

            await _pacienteRepository.AddAsync(paciente);
        }

        public async Task UpdateAsync(PacienteDTO pacienteDTO)
        {
            var paciente = new Paciente
            {
                Id = pacienteDTO.Id,
                DataNascimento = pacienteDTO.DataNascimento,
                Telefone = pacienteDTO.Telefone,
                Endereco = pacienteDTO.Endereco,
                UsuarioId = pacienteDTO.UsuarioId
            };

            await _pacienteRepository.UpdateAsync(paciente);
        }

        public async Task DeleteAsync(int id)
        {
            await _pacienteRepository.DeleteAsync(id);
        }
    }
}