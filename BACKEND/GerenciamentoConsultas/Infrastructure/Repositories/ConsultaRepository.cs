using Dapper;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using System.Data;

namespace GerenciamentoConsultas.Infrastructure.Repositories
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly IDbConnection _dbConnection;

        public ConsultaRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Consulta?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT * 
                FROM ""Consultas"" 
                WHERE ""Id"" = @Id";

            var consulta = await _dbConnection.QueryFirstOrDefaultAsync<Consulta>(query, new { Id = id });

            if (consulta != null)
            {
                var pacienteQuery = @"
                    SELECT * 
                    FROM ""Pacientes"" 
                    WHERE ""Id"" = @PacienteId";

                consulta.Paciente = await _dbConnection.QueryFirstOrDefaultAsync<Paciente>(pacienteQuery, new { consulta.PacienteId });

                var medicoQuery = @"
                    SELECT * 
                    FROM ""Medicos"" 
                    WHERE ""Id"" = @MedicoId";

                consulta.Medico = await _dbConnection.QueryFirstOrDefaultAsync<Medico>(medicoQuery, new { consulta.MedicoId });
            }

            return consulta;
        }

        public async Task<IEnumerable<Consulta>> GetAllAsync()
        {
            var query = "SELECT * FROM \"Consultas\"";
            var consultas = await _dbConnection.QueryAsync<Consulta>(query);

            foreach (var consulta in consultas)
            {
                var pacienteQuery = @"
                    SELECT * 
                    FROM ""Pacientes"" 
                    WHERE ""Id"" = @PacienteId";

                consulta.Paciente = await _dbConnection.QueryFirstOrDefaultAsync<Paciente>(pacienteQuery, new { consulta.PacienteId });

                var medicoQuery = @"
                    SELECT * 
                    FROM ""Medicos"" 
                    WHERE ""Id"" = @MedicoId";

                consulta.Medico = await _dbConnection.QueryFirstOrDefaultAsync<Medico>(medicoQuery, new { consulta.MedicoId });
            }

            return consultas;
        }

        public async Task AddAsync(Consulta consulta)
        {
            var query = @"
                INSERT INTO ""Consultas"" 
                (""PacienteId"", ""MedicoId"", ""DataHora"", ""Status"", ""Observacoes"") 
                VALUES (@PacienteId, @MedicoId, @DataHora, @Status, @Observacoes)";

            await _dbConnection.ExecuteAsync(query, new
            {
                consulta.PacienteId,
                consulta.MedicoId,
                consulta.DataHora,
                consulta.Status,
                consulta.Observacoes
            });
        }

        public async Task UpdateAsync(Consulta consulta)
        {
            var query = @"
        UPDATE ""Consultas"" 
        SET 
            ""PacienteId"" = @PacienteId, 
            ""MedicoId"" = @MedicoId, 
            ""DataHora"" = @DataHora, 
            ""Status"" = @Status, 
            ""Observacoes"" = @Observacoes 
        WHERE ""Id"" = @Id";

            await _dbConnection.ExecuteAsync(query, new
            {
                consulta.PacienteId,
                consulta.MedicoId,
                consulta.DataHora,
                consulta.Status,
                consulta.Observacoes,
                consulta.Id 
            });
        }


        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM \"Consultas\" WHERE \"Id\" = @Id";
            await _dbConnection.ExecuteAsync(query, new { Id = id });
        }
    }
}
