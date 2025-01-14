using Dapper;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoConsultas.Tests.Repositories
{
    public class UsuarioRepositoryTests
    {
        private readonly Mock<IDbConnection> _mockDbConnection;
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioRepositoryTests()
        {
            _mockDbConnection = new Mock<IDbConnection>();
            _usuarioRepository = new UsuarioRepository(_mockDbConnection.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUsuarioWithMedicoAndPaciente_WhenTheyExist()
        {
            // Arrange
            var usuarioId = 1;

            var query = @"
    SELECT 
        u.""Id"" AS UsuarioId, u.""Nome"", u.""Email"", u.""Senha"", u.""TipoUsuario"", u.""DataCadastro"",
        m.""Id"" AS MedicoId, m.""Especialidade"", m.""CRM"", m.""UsuarioId"" AS MedicoUsuarioId,
        p.""Id"" AS PacienteId, p.""DataNascimento"", p.""Telefone"", p.""Endereco"", p.""UsuarioId"" AS PacienteUsuarioId
    FROM ""Usuarios"" u
    LEFT JOIN ""Medicos"" m ON u.""Id"" = m.""UsuarioId""
    LEFT JOIN ""Pacientes"" p ON u.""Id"" = p.""UsuarioId""
    WHERE u.""Id"" = @Id";

            var queryResult = new[]
            {
                new
                {
                    UsuarioId = 1,
                    Nome = "Teste",
                    Email = "teste@teste.com",
                    Senha = "123456",
                    TipoUsuario = (int)TipoUsuario.Administrador,
                    DataCadastro = DateTime.Parse("2025-01-01"),
                    MedicoId = 10,
                    Especialidade = "Cardiologia",
                    CRM = "123456-SP",
                    MedicoUsuarioId = 1,
                    PacienteId = 20,
                    DataNascimento = DateTime.Parse("1990-01-01"),
                    Telefone = "123456789",
                    Endereco = "Rua Teste",
                    PacienteUsuarioId = 1
                }
            };

            // Configurando o Mock do DbConnection
            _mockDbConnection
                .Setup(db => db.QueryAsync<Usuario, Medico, Paciente, Usuario>(
                    query, // Passando a query como argumento
                    It.IsAny<Func<Usuario, Medico, Paciente, Usuario>>(), // Função de mapeamento
                    new { Id = usuarioId }, // Parâmetros para a query
                    It.IsAny<IDbTransaction>(), // Transação (não estamos usando no momento)
                    null, // Timeout (pode ser null)
                    "MedicoId,PacienteId" // Split columns
                ))
                .ReturnsAsync((string sql, Func<Usuario, Medico, Paciente, Usuario> map, object param, IDbTransaction transaction, int? commandTimeout, string splitOn) =>
                {
                    var usuarios = new List<Usuario>();
                    foreach (var item in queryResult)
                    {
                        var usuario = new Usuario
                        {
                            Id = item.UsuarioId,
                            Nome = item.Nome,
                            Email = item.Email,
                            Senha = item.Senha,
                            TipoUsuario = (TipoUsuario)item.TipoUsuario,
                            DataCadastro = item.DataCadastro,
                        };

                        var medico = new Medico
                        {
                            Id = item.MedicoId,
                            Especialidade = item.Especialidade,
                            CRM = item.CRM,
                            UsuarioId = item.MedicoUsuarioId
                        };

                        var paciente = new Paciente
                        {
                            Id = item.PacienteId,
                            DataNascimento = item.DataNascimento,
                            Telefone = item.Telefone,
                            Endereco = item.Endereco,
                            UsuarioId = item.PacienteUsuarioId
                        };

                        usuarios.Add(map(usuario, medico, paciente));
                    }
                    return usuarios;
                });

            // Act
            var result = await _usuarioRepository.GetByIdAsync(usuarioId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuarioId, result.Id);
            Assert.NotNull(result.Medico);
            Assert.Equal(10, result.Medico.Id);
            Assert.NotNull(result.Paciente);
            Assert.Equal(20, result.Paciente.Id);
        }
    }
}
