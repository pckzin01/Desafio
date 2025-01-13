using Dapper;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GerenciamentoConsultas.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _dbConnection;

        public UsuarioRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var query = @"
    SELECT 
        u.""Id"" AS UsuarioId, u.""Nome"", u.""Email"", u.""Senha"", u.""TipoUsuario"", u.""DataCadastro"",
        m.""Id"" AS MedicoId, m.""Especialidade"", m.""CRM"", m.""UsuarioId"" AS MedicoUsuarioId,
        p.""Id"" AS PacienteId, p.""DataNascimento"", p.""Telefone"", p.""Endereco"", p.""UsuarioId"" AS PacienteUsuarioId
    FROM ""Usuarios"" u
    LEFT JOIN ""Medicos"" m ON u.""Id"" = m.""UsuarioId""
    LEFT JOIN ""Pacientes"" p ON u.""Id"" = p.""UsuarioId""
    WHERE u.""Id"" = @Id";

            var result = await _dbConnection.QueryAsync<Usuario, Medico, Paciente, Usuario>(
                query,
                (usuario, medico, paciente) =>
                {
                    // Verificar se há um médico válido e associá-lo ao usuário
                    if (medico != null && medico.Id > 0)
                        usuario.Medico = medico;

                    // Verificar se há um paciente válido e associá-lo ao usuário
                    if (paciente != null && paciente.Id > 0)
                        usuario.Paciente = paciente;

                    return usuario;
                },
                new { Id = id },
                splitOn: "MedicoId,PacienteId"
            );

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var query = "SELECT * FROM \"Usuarios\"";
            return await _dbConnection.QueryAsync<Usuario>(query);
        }

        public async Task<int> AddAsync(Usuario usuario)
        {
            var query = @"
        INSERT INTO ""Usuarios"" (""Nome"", ""Email"", ""Senha"", ""TipoUsuario"", ""DataCadastro"")
        VALUES (@Nome, @Email, @Senha, @TipoUsuario, @DataCadastro)
        RETURNING ""Id""";

            var id = await _dbConnection.ExecuteScalarAsync<int>(query, usuario);
            return id;
        }


        public async Task UpdateAsync(Usuario usuario)
        {
            var query = "UPDATE \"Usuarios\" SET \"Nome\" = @Nome, \"Email\" = @Email, \"Senha\" = @Senha, \"TipoUsuario\" = @TipoUsuario WHERE \"Id\" = @Id";
            await _dbConnection.ExecuteAsync(query, usuario);
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM \"Usuarios\" WHERE \"Id\" = @Id";
            await _dbConnection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM \"Usuarios\" WHERE \"Email\" = @Email";
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM \"Usuarios\" WHERE \"Id\" = @Id";
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }

    }
}
