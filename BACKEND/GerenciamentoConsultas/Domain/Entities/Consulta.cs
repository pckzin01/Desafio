

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoConsultas.Domain.Entities
{
    public class Consulta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }
        public int MedicoId { get; set; }
        public Medico? Medico { get; set; }
        public DateTime DataHora { get; set; }
        public StatusConsulta Status { get; set; }
        public string? Observacoes { get; set; }
    }

    public enum StatusConsulta
    {
        Agendada = 1,
        Realizada = 2,
        Cancelada =  3
    }
}
