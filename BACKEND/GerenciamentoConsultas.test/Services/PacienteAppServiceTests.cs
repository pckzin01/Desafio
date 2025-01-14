using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoConsultas.Tests
{
    public class PacienteAppServiceTests
    {
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly PacienteAppService _pacienteAppService;

        public PacienteAppServiceTests()
        {
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _pacienteAppService = new PacienteAppService(_pacienteRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPaciente_WhenPacienteExists()
        {
            // Arrange
            var paciente = new Paciente
            {
                Id = 1,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "123456789",
                Endereco = "Rua A, 123",
                UsuarioId = 1
            };

            _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(paciente);

            // Act
            var result = await _pacienteAppService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paciente.Id, result.Id);
            Assert.Equal(paciente.DataNascimento, result.DataNascimento);
            Assert.Equal(paciente.Telefone, result.Telefone);
            Assert.Equal(paciente.Endereco, result.Endereco);
            Assert.Equal(paciente.UsuarioId, result.UsuarioId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPacienteDoesNotExist()
        {
            // Arrange
            _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Paciente?)null);

            // Act
            var result = await _pacienteAppService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfPacientes()
        {
            // Arrange
            var pacientes = new List<Paciente>
            {
                new Paciente { Id = 1, DataNascimento = new DateTime(1990, 1, 1), Telefone = "123456789", Endereco = "Rua A, 123", UsuarioId = 1 },
                new Paciente { Id = 2, DataNascimento = new DateTime(1985, 5, 15), Telefone = "987654321", Endereco = "Rua B, 456", UsuarioId = 2 }
            };

            _pacienteRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(pacientes);

            // Act
            var result = await _pacienteAppService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddPaciente()
        {
            // Arrange
            var pacienteDTO = new PacienteDTO
            {
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "123456789",
                Endereco = "Rua A, 123",
                UsuarioId = 1
            };

            _pacienteRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

            // Act
            await _pacienteAppService.AddAsync(pacienteDTO);

            // Assert
            _pacienteRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Paciente>(p =>
                p.DataNascimento == pacienteDTO.DataNascimento &&
                p.Telefone == pacienteDTO.Telefone &&
                p.Endereco == pacienteDTO.Endereco &&
                p.UsuarioId == pacienteDTO.UsuarioId
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePaciente()
        {
            // Arrange
            var pacienteDTO = new PacienteDTO
            {
                Id = 1,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "123456789",
                Endereco = "Rua A, 123",
                UsuarioId = 1
            };

            _pacienteRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

            // Act
            await _pacienteAppService.UpdateAsync(pacienteDTO);

            // Assert
            _pacienteRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Paciente>(p =>
                p.Id == pacienteDTO.Id &&
                p.DataNascimento == pacienteDTO.DataNascimento &&
                p.Telefone == pacienteDTO.Telefone &&
                p.Endereco == pacienteDTO.Endereco &&
                p.UsuarioId == pacienteDTO.UsuarioId
            )), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePaciente()
        {
            // Arrange
            var pacienteId = 1;

            _pacienteRepositoryMock.Setup(repo => repo.DeleteAsync(pacienteId)).Returns(Task.CompletedTask);

            // Act
            await _pacienteAppService.DeleteAsync(pacienteId);

            // Assert
            _pacienteRepositoryMock.Verify(repo => repo.DeleteAsync(pacienteId), Times.Once);
        }
    }
}
