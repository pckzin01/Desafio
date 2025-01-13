using System;
using System.ComponentModel.DataAnnotations;
using GerenciamentoConsultas.Domain.Entities;
using BCrypt.Net;

public class UsuarioInserirDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O email não é válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
    public TipoUsuario TipoUsuario { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public void HashSenha()
    {
        Senha = BCrypt.Net.BCrypt.HashPassword(Senha);
    }
}
