using Dapper;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace GerenciamentoConsultas.Infrastructure.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly IDbConnection _dbConnection;

        public MedicoRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Medico?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT * 
                FROM ""Medicos"" 
                WHERE ""Id"" = @Id";

            var medico = await _dbConnection.QueryFirstOrDefaultAsync<Medico>(query, new { Id = id });

            if (medico != null)
            {
                var usuarioQuery = @"
                    SELECT * 
                    FROM ""Usuarios"" 
                    WHERE ""Id"" = @UsuarioId";

                medico.Usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(usuarioQuery, new { UsuarioId = medico.UsuarioId });

                var consultasQuery = @"
                    SELECT * 
                    FROM ""Consultas"" 
                    WHERE ""MedicoId"" = @MedicoId";

                medico.Consultas = (await _dbConnection.QueryAsync<Consulta>(consultasQuery, new { MedicoId = id })).AsList();
            }

            return medico;
        }

        public async Task<IEnumerable<Medico>> GetAllAsync()
        {
            var query = "SELECT * FROM \"Medicos\"";
            var medicos = await _dbConnection.QueryAsync<Medico>(query);

            foreach (var medico in medicos)
            {
                var usuarioQuery = @"
                    SELECT * 
                    FROM ""Usuarios"" 
                    WHERE ""Id"" = @UsuarioId";

                medico.Usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(usuarioQuery, new { UsuarioId = medico.UsuarioId });

                var consultasQuery = @"
                    SELECT * 
                    FROM ""Consultas"" 
                    WHERE ""MedicoId"" = @MedicoId";

                medico.Consultas = (await _dbConnection.QueryAsync<Consulta>(consultasQuery, new { MedicoId = medico.Id })).AsList();
            }

            return medicos;
        }

        public async Task AddAsync(Medico medico)
        {
            var query = @"
                INSERT INTO ""Medicos"" (""Especialidade"", ""CRM"", ""UsuarioId"") 
                VALUES (@Especialidade, @CRM, @UsuarioId)";

            await _dbConnection.ExecuteAsync(query, new
            {
                medico.Especialidade,
                medico.CRM,
                medico.UsuarioId
            });
        }

        public async Task UpdateAsync(Medico medico)
        {
            var query = @"
                UPDATE ""Medicos"" 
                SET ""Especialidade"" = @Especialidade, 
                    ""CRM"" = @CRM, 
                    ""UsuarioId"" = @UsuarioId 
                WHERE ""Id"" = @Id";

            await _dbConnection.ExecuteAsync(query, new
            {
                medico.Especialidade,
                medico.CRM,
                medico.UsuarioId,
                medico.Id
            });
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM \"Medicos\" WHERE \"Id\" = @Id";
            await _dbConnection.ExecuteAsync(query, new { Id = id });
        }

        public async Task DeleteAsync(Medico medico)
        {
            await DeleteAsync(medico.Id);
        }
    }
}
