using GerenciamentoConsultas.Domain.Entities;

namespace GerenciamentoConsultas.Domain.Interfaces
{
    public interface IMedicoRepository
    {
        Task<Medico?> GetByIdAsync(int id);
        Task<IEnumerable<Medico>> GetAllAsync();
        Task AddAsync(Medico medico);
        Task UpdateAsync(Medico medico);
        Task DeleteAsync(int id);
    }
}
