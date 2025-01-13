namespace GerenciamentoConsultas.Application.DTOs
{
    public class ConsultaDTO
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public DateTime DataHora { get; set; }
        public StatusConsulta Status { get; set; }
        public string? Observacoes { get; set; }
        public int PacienteId { get; set; }
    }

    public enum StatusConsulta
    {
        Agendada = 1,
        Realizada = 2,
        Cancelada = 3,
        consulta = 4
    }
}
