using GerenciamentoConsultas.Domain.Entities;

namespace GerenciamentoConsultas.Domain.Interfaces
{
    public interface IPacienteRepository
    {
        Task AddAsync(Paciente paciente);
        Task<Paciente?> GetByIdAsync(int id);
        Task<IEnumerable<Paciente>> GetAllAsync();
        Task UpdateAsync(Paciente paciente);
        Task DeleteAsync(int id);
    }
}