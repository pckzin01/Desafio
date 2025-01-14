using Xunit;
using Moq;
using GerenciamentoConsultas.Application.DTOs;
using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Domain.Entities;
using GerenciamentoConsultas.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UsuarioAppServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly UsuarioAppService _usuarioAppService;

    public UsuarioAppServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _usuarioAppService = new UsuarioAppService(_usuarioRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUsuarioByIdAsync_DeveRetornarUsuario_QuandoUsuarioExistir()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 1,
            Nome = "Teste Usuário",
            Email = "teste@exemplo.com",
            TipoUsuario = TipoUsuario.Paciente
        };

        _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);

        // Act
        var result = await _usuarioAppService.GetUsuarioByIdAsync(usuario.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuario.Id, result.Id);
        Assert.Equal(usuario.Nome, result.Nome);
        Assert.Equal(usuario.Email, result.Email);
        Assert.Equal(usuario.TipoUsuario, result.TipoUsuario);
    }

    [Fact]
    public async Task GetUsuarioByIdAsync_DeveLancarExcecao_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var usuarioId = 999;
        _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync((Usuario)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioAppService.GetUsuarioByIdAsync(usuarioId));
    }

    [Fact]
    public async Task AddUsuarioAsync_DeveAdicionarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var usuarioInserirDTO = new UsuarioInserirDTO
        {
            Nome = "Novo Usuário",
            Email = "novo@exemplo.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Paciente,
            DataCadastro = DateTime.UtcNow
        };

        _usuarioRepositoryMock.Setup(repo => repo.EmailExistsAsync(usuarioInserirDTO.Email)).ReturnsAsync(false);
        _usuarioRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Usuario>())).ReturnsAsync(1);

        // Act
        var usuarioId = await _usuarioAppService.AddUsuarioAsync(usuarioInserirDTO);

        // Assert
        Assert.Equal(1, usuarioId);
        _usuarioRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task AddUsuarioAsync_DeveLancarExcecao_QuandoEmailJaExistir()
    {
        // Arrange
        var usuarioInserirDTO = new UsuarioInserirDTO
        {
            Nome = "Usuário Existente",
            Email = "existente@exemplo.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Paciente,
            DataCadastro = DateTime.UtcNow
        };

        _usuarioRepositoryMock.Setup(repo => repo.EmailExistsAsync(usuarioInserirDTO.Email)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _usuarioAppService.AddUsuarioAsync(usuarioInserirDTO));
    }

    [Fact]
    public async Task UpdateUsuarioAsync_DeveAtualizarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var usuarioDTO = new UsuarioDTO
        {
            Id = 1,
            Nome = "Usuário Atualizado",
            Email = "atualizado@exemplo.com",
            TipoUsuario = TipoUsuario.Medico
        };

        var usuarioExistente = new Usuario
        {
            Id = usuarioDTO.Id,
            Nome = "Usuário Antigo",
            Email = "antigo@exemplo.com",
            TipoUsuario = TipoUsuario.Paciente
        };

        _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(usuarioDTO.Id)).ReturnsAsync(usuarioExistente);

        // Act
        await _usuarioAppService.UpdateUsuarioAsync(usuarioDTO);

        // Assert
        _usuarioRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Usuario>(u =>
            u.Id == usuarioDTO.Id &&
            u.Nome == usuarioDTO.Nome &&
            u.Email == usuarioDTO.Email &&
            u.TipoUsuario == usuarioDTO.TipoUsuario
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteUsuarioAsync_DeveExcluirUsuario_QuandoUsuarioExistir()
    {
        // Arrange
        var usuarioId = 1;

        var usuarioExistente = new Usuario
        {
            Id = usuarioId,
            Nome = "Usuário a Excluir",
            Email = "excluir@exemplo.com",
            TipoUsuario = TipoUsuario.Paciente
        };

        _usuarioRepositoryMock.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync(usuarioExistente);

        // Act
        await _usuarioAppService.DeleteUsuarioAsync(usuarioId);

        // Assert
        _usuarioRepositoryMock.Verify(repo => repo.DeleteAsync(usuarioId), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_DeveRetornarUsuario_QuandoCredenciaisForemValidas()
    {
        // Arrange
        var email = "valido@exemplo.com";
        var senha = "senha123";

        var usuario = new Usuario
        {
            Id = 1,
            Nome = "Usuário",
            Email = email,
            Senha = BCrypt.Net.BCrypt.HashPassword(senha),
            TipoUsuario = TipoUsuario.Paciente
        };

        _usuarioRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Usuario> { usuario });

        // Act
        var result = await _usuarioAppService.AuthenticateAsync(email, senha);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuario.Id, result.Id);
        Assert.Equal(usuario.Nome, result.Nome);
        Assert.Equal(usuario.Email, result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_DeveRetornarNulo_QuandoCredenciaisForemInvalidas()
    {
        // Arrange
        var email = "invalido@exemplo.com";
        var senha = "senhaErrada";

        _usuarioRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Usuario>());

        // Act
        var result = await _usuarioAppService.AuthenticateAsync(email, senha);

        // Assert
        Assert.Null(result);
    }
}
