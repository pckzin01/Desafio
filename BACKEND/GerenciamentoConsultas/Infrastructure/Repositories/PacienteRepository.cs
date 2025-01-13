namespace GerenciamentoConsultas.Infraestrutura.Repositories
{
    using Dapper;
    using System.Data;
    using GerenciamentoConsultas.Domain.Entities;
    using GerenciamentoConsultas.Domain.Interfaces;

    public class PacienteRepository : IPacienteRepository
    {
        private readonly IDbConnection _dbConnection;

        public PacienteRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task AddAsync(Paciente paciente)
        {
            var query = @"
                INSERT INTO ""Pacientes"" (""DataNascimento"", ""Telefone"", ""Endereco"", ""UsuarioId"")
                VALUES (@DataNascimento, @Telefone, @Endereco, @UsuarioId)
                RETURNING ""Id"";";

            paciente.Id = await _dbConnection.ExecuteScalarAsync<int>(query, paciente);
        }

        public async Task<Paciente?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT *
                FROM ""Pacientes""
                WHERE ""Id"" = @Id;";

            return await _dbConnection.QuerySingleOrDefaultAsync<Paciente>(query, new { Id = id });
        }

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            var query = @"SELECT * FROM ""Pacientes"";";
            return await _dbConnection.QueryAsync<Paciente>(query);
        }

        public async Task UpdateAsync(Paciente paciente)
        {
            var query = @"
                UPDATE ""Pacientes""
                SET ""DataNascimento"" = @DataNascimento, 
                    ""Telefone"" = @Telefone, 
                    ""Endereco"" = @Endereco, 
                    ""UsuarioId"" = @UsuarioId
                WHERE ""Id"" = @Id;";

            await _dbConnection.ExecuteAsync(query, paciente);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"DELETE FROM ""Pacientes"" WHERE ""Id"" = @Id;";
            await _dbConnection.ExecuteAsync(query, new { Id = id });
        }
    }
}
