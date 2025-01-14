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

namespace GerenciamentoConsultas.Tests
{
    public class MedicoAppServiceTests
    {
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly MedicoAppService _medicoAppService;

        public MedicoAppServiceTests()
        {
            _medicoRepositoryMock = new Mock<IMedicoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _medicoAppService = new MedicoAppService(_medicoRepositoryMock.Object, _usuarioRepositoryMock.Object);
        }

        [Fact]
        public async Task GetMedicoByIdAsync_ShouldReturnMedico_WhenMedicoExists()
        {
            // Arrange
            var medico = new Medico
            {
                Id = 1,
                Especialidade = "Cardiologia",
                CRM = "12345-MG",
                UsuarioId = 2
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(medico);

            // Act
            var result = await _medicoAppService.GetMedicoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medico.Id, result.Id);
            Assert.Equal(medico.Especialidade, result.Especialidade);
            Assert.Equal(medico.CRM, result.CRM);
            Assert.Equal(medico.UsuarioId, result.UsuarioId);
        }

        [Fact]
        public async Task GetMedicoByIdAsync_ShouldThrowKeyNotFoundException_WhenMedicoDoesNotExist()
        {
            // Arrange
            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Medico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _medicoAppService.GetMedicoByIdAsync(1));
        }

        [Fact]
        public async Task GetAllMedicosAsync_ShouldReturnListOfMedicos()
        {
            // Arrange
            var medicos = new List<Medico>
            {
                new Medico { Id = 1, Especialidade = "Cardiologia", CRM = "12345-MG", UsuarioId = 2 },
                new Medico { Id = 2, Especialidade = "Dermatologia", CRM = "67890-SP", UsuarioId = 3 }
            };

            _medicoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(medicos);

            // Act
            var result = await _medicoAppService.GetAllMedicosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddMedicoAsync_ShouldThrowArgumentException_WhenCRMIsInvalid()
        {
            // Arrange
            var medicoDTO = new MedicoDTO
            {
                Especialidade = "Cardiologia",
                CRM = "12345SP",
                UsuarioId = 2
            };

            _usuarioRepositoryMock.Setup(repo => repo.ExistsAsync(medicoDTO.UsuarioId)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _medicoAppService.AddMedicoAsync(medicoDTO);
            });

            Assert.Equal("CRM inválido. Verifique o formato e a sigla do estado.", exception.Message);

            _medicoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Fact]
        public async Task AddMedicoAsync_ShouldThrowArgumentException_WhenCrmIsInvalid()
        {
            // Arrange
            var medicoDTO = new MedicoDTO
            {
                Especialidade = "Cardiologia",
                CRM = "INVALID",
                UsuarioId = 2
            };

            _usuarioRepositoryMock.Setup(repo => repo.ExistsAsync(medicoDTO.UsuarioId)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _medicoAppService.AddMedicoAsync(medicoDTO));
        }

        [Fact]
        public async Task UpdateMedicoAsync_ShouldUpdateMedico_WhenMedicoExists()
        {
            // Arrange
            var medicoDTO = new MedicoDTO
            {
                Id = 1,
                Especialidade = "Pediatria",
                CRM = "54321-MG",
                UsuarioId = 2
            };

            var existingMedico = new Medico
            {
                Id = 1,
                Especialidade = "Cardiologia",
                CRM = "12345-MG",
                UsuarioId = 2
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoDTO.Id)).ReturnsAsync(existingMedico);
            _usuarioRepositoryMock.Setup(repo => repo.ExistsAsync(medicoDTO.UsuarioId)).ReturnsAsync(true);

            // Act
            await _medicoAppService.UpdateMedicoAsync(medicoDTO);

            // Assert
            _medicoRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Medico>(m =>
                m.Id == medicoDTO.Id &&
                m.Especialidade == medicoDTO.Especialidade &&
                m.CRM == medicoDTO.CRM &&
                m.UsuarioId == medicoDTO.UsuarioId
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicoAsync_ShouldThrowKeyNotFoundException_WhenMedicoDoesNotExist()
        {
            // Arrange
            var medicoDTO = new MedicoDTO
            {
                Id = 1,
                Especialidade = "Pediatria",
                CRM = "54321-MG",
                UsuarioId = 2
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoDTO.Id)).ReturnsAsync((Medico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _medicoAppService.UpdateMedicoAsync(medicoDTO));
        }

        [Fact]
        public async Task DeleteMedicoAsync_ShouldDeleteMedico_WhenMedicoExists()
        {
            // Arrange
            var medicoId = 1;
            var existingMedico = new Medico
            {
                Id = medicoId,
                Especialidade = "Cardiologia",
                CRM = "12345-MG",
                UsuarioId = 2
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoId)).ReturnsAsync(existingMedico);

            // Act
            await _medicoAppService.DeleteMedicoAsync(medicoId);

            // Assert
            _medicoRepositoryMock.Verify(repo => repo.DeleteAsync(medicoId), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicoAsync_ShouldThrowKeyNotFoundException_WhenMedicoDoesNotExist()
        {
            // Arrange
            var medicoId = 1;

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoId)).ReturnsAsync((Medico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _medicoAppService.DeleteMedicoAsync(medicoId));
        }
    }
}
