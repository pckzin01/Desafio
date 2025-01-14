using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using Xunit;
using ApplicationStatusConsulta = GerenciamentoConsultas.Application.DTOs.StatusConsulta;
using DomainStatusConsulta = GerenciamentoConsultas.Domain.Entities.StatusConsulta;


namespace GerenciamentoConsultas.Tests
{
    public class ConsultaAppServiceTests
    {
        private readonly Mock<IConsultaRepository> _consultaRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly ConsultaAppService _consultaAppService;

        public ConsultaAppServiceTests()
        {
            _consultaRepositoryMock = new Mock<IConsultaRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _medicoRepositoryMock = new Mock<IMedicoRepository>();
            _consultaAppService = new ConsultaAppService(_consultaRepositoryMock.Object, _usuarioRepositoryMock.Object, _medicoRepositoryMock.Object);
        }

        [Fact]
        public async Task AddConsultaAsync_ShouldAddConsulta_WhenValidData()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                PacienteId = 1,
                MedicoId = 2,
                DataHora = DateTime.Now.AddHours(1),
                Status = ApplicationStatusConsulta.Agendada,  // Usando alias
                Observacoes = "Consulta de rotina"
            };

            // Mock para o repositório de usuário e médico
            _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId)).ReturnsAsync(new Usuario { Id = consultaDTO.PacienteId });
            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.MedicoId)).ReturnsAsync(new Medico { Id = consultaDTO.MedicoId });

            _consultaRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Consulta>())).Returns(Task.CompletedTask);

            // Act
            await _consultaAppService.AddConsultaAsync(consultaDTO);

            // Assert
            _consultaRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Consulta>(c =>
                c.PacienteId == consultaDTO.PacienteId &&
                c.MedicoId == consultaDTO.MedicoId &&
                c.DataHora == consultaDTO.DataHora &&
                c.Status == (DomainStatusConsulta)consultaDTO.Status &&  // Usando alias
                c.Observacoes == consultaDTO.Observacoes
            )), Times.Once);
        }

        [Fact]
        public async Task AddConsultaAsync_ShouldThrowException_WhenPacienteNotFound()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                PacienteId = 1,
                MedicoId = 2,
                DataHora = DateTime.Now.AddHours(1),
                Status = ApplicationStatusConsulta.Agendada,  // Usando alias
                Observacoes = "Consulta de rotina"
            };

            // Mock para o repositório de usuário e médico
            _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId)).ReturnsAsync((Usuario)null);
            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.MedicoId)).ReturnsAsync(new Medico { Id = consultaDTO.MedicoId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _consultaAppService.AddConsultaAsync(consultaDTO));
            Assert.Equal("Paciente não encontrado.", exception.Message);
        }

        [Fact]
        public async Task AddConsultaAsync_ShouldThrowException_WhenMedicoNotFound()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO
            {
                PacienteId = 1,
                MedicoId = 2,
                DataHora = DateTime.Now.AddHours(1),
                Status = ApplicationStatusConsulta.Agendada,  // Usando alias
                Observacoes = "Consulta de rotina"
            };

            // Mock para o repositório de usuário e médico
            _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.PacienteId)).ReturnsAsync(new Usuario { Id = consultaDTO.PacienteId });
            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.MedicoId)).ReturnsAsync((Medico)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _consultaAppService.AddConsultaAsync(consultaDTO));
            Assert.Equal("Médico não encontrado.", exception.Message);
        }

        [Fact]
        public async Task UpdateConsultaAsync_ShouldThrowException_WhenConsultaNotFound()
        {
            // Arrange
            var consultaDTO = new ConsultaDTO { Id = 1, PacienteId = 1, MedicoId = 2, DataHora = DateTime.Now.AddHours(1), Status = ApplicationStatusConsulta.Agendada }; // Usando alias

            // Mock para o repositório de consulta, paciente e médico
            _consultaRepositoryMock.Setup(repo => repo.GetByIdAsync(consultaDTO.Id)).ReturnsAsync((Consulta)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _consultaAppService.UpdateConsultaAsync(consultaDTO));
            Assert.Equal("Consulta não encontrada.", exception.Message);
        }
    }
}
