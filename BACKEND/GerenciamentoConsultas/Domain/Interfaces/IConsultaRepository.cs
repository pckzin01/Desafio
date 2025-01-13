using GerenciamentoConsultas.Domain.Entities;

namespace GerenciamentoConsultas.Domain.Interfaces
{
    public interface IConsultaRepository
    {
        Task<Consulta?> GetByIdAsync(int id);
        Task<IEnumerable<Consulta>> GetAllAsync();
        Task AddAsync(Consulta consulta);
        Task UpdateAsync(Consulta consulta);
        Task DeleteAsync(int id);
    }
}
