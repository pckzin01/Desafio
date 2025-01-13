namespace GerenciamentoConsultas.Application.DTOs
{
    public class PacienteDTO
    {
        public int Id { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}