namespace GerenciamentoConsultas.Application.DTOs
{
    public class MedicoDTO
    {
        public int Id { get; set; }
        public string Especialidade { get; set; } = string.Empty;
        public string CRM { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}
