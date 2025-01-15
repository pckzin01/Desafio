using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoConsultas.Tests.Services
{
    public class ConsultaAppServiceTests
    {
        private readonly Mock<IConsultaRepository> _consultaRepositoryMock;
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly ConsultaAppService _consultaAppService;

        public ConsultaAppServiceTests()
        {
            _consultaRepositoryMock = new Mock<IConsultaRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _medicoRepositoryMock = new Mock<IMedicoRepository>();

            _consultaAppService = new ConsultaAppService(
                _consultaRepositoryMock.Object,
                _pacienteRepositoryMock.Object,
                _medicoRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetConsultaByIdAsync_ShouldReturnConsultaDTO_WhenConsultaExists()
        {
            // Arrange
            var consultaId = 1;
            var consulta = new Consulta
            {
                Id = consultaId,
                MedicoId = 2,
                PacienteId = 3,
                DataHora = DateTime.Now,
                Status = Domain.Entities.StatusConsulta.Agendada,
                Observacoes = "Teste"
            };

            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaId))
                .ReturnsAsync(consulta);

            // Act
            var result = await _consultaAppService.GetConsultaByIdAsync(consultaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(consulta.Id, result.Id);
            Assert.Equal(consulta.MedicoId, result.MedicoId);
            Assert.Equal(consulta.PacienteId, result.PacienteId);
            Assert.Equal(consulta.DataHora, result.DataHora);
            Assert.Equal((Application.DTOs.StatusConsulta)consulta.Status, result.Status);
        }

        [Fact]
        public async Task GetConsultaByIdAsync_ShouldThrowException_WhenConsultaDoesNotExist()
        {
            // Arrange
            var consultaId = 1;
            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaId))
                .ReturnsAsync((Consulta)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _consultaAppService.GetConsultaByIdAsync(consultaId));
        }

        [Fact]
        public async Task AddConsultaAsync_ShouldCallRepository_WhenValid()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                MedicoId = 1,
                PacienteId = 2,
                DataHora = DateTime.Now,
                Status = Application.DTOs.StatusConsulta.Agendada,
                Observacoes = "Teste"
            };

            _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId))
                .ReturnsAsync(new Paciente { Id = consultaDTO.PacienteId });

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.MedicoId))
                .ReturnsAsync(new Medico { Id = consultaDTO.MedicoId });

            // Act
            await _consultaAppService.AddConsultaAsync(consultaDTO);

            // Assert
            _consultaRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Consulta>()), Times.Once);
        }

        [Fact]
        public async Task AddConsultaAsync_ShouldThrowException_WhenPacienteDoesNotExist()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                MedicoId = 1,
                PacienteId = 2,
                DataHora = DateTime.Now,
                Status = Application.DTOs.StatusConsulta.Agendada,
                Observacoes = "Teste"
            };

            _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId))
                .ReturnsAsync((Paciente)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _consultaAppService.AddConsultaAsync(consultaDTO));
        }

        [Fact]
        public async Task UpdateConsultaAsync_ShouldUpdateConsulta_WhenValid()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                Id = 1,
                MedicoId = 2,
                PacienteId = 3,
                DataHora = DateTime.Now,
                Status = Application.DTOs.StatusConsulta.Agendada,
                Observacoes = "Atualizado"
            };

            var consulta = new Consulta
            {
                Id = consultaDTO.Id,
                MedicoId = consultaDTO.MedicoId,
                PacienteId = consultaDTO.PacienteId,
                DataHora = consultaDTO.DataHora,
                Status = Domain.Entities.StatusConsulta.Agendada,
                Observacoes = "Teste"
            };

            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.Id))
                .ReturnsAsync(consulta);

            _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId))
                .ReturnsAsync(new Paciente { Id = consultaDTO.PacienteId });

            // Act
            await _consultaAppService.UpdateConsultaAsync(consultaDTO);

            // Assert
            _consultaRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Consulta>(c => c.Id == consultaDTO.Id)), Times.Once);
        }

        [Fact]
        public async Task DeleteConsultaAsync_ShouldCallRepository_WhenConsultaExists()
        {
            // Arrange
            var consultaId = 1;
            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaId))
                .ReturnsAsync(new Consulta { Id = consultaId });

            // Act
            await _consultaAppService.DeleteConsultaAsync(consultaId);

            // Assert
            _consultaRepositoryMock.Verify(repo => repo.DeleteAsync(consultaId), Times.Once);
        }

        [Fact]
        public async Task DeleteConsultaAsync_ShouldThrowException_WhenConsultaDoesNotExist()
        {
            // Arrange
            var consultaId = 1;
            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaId))
                .ReturnsAsync((Consulta)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _consultaAppService.DeleteConsultaAsync(consultaId));
        }
    }
}
