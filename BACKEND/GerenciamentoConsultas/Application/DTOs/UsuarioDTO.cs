using GerenciamentoConsultas.Domain.Entities;

namespace GerenciamentoConsultas.Application.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime DataCadastro { get; set; }
        public MedicoDTO? Medico { get; set; }
        public PacienteDTO? Paciente { get; set; }
    }
}
