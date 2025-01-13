

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoConsultas.Domain.Entities
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public Medico? Medico { get; set; }
        public Paciente? Paciente { get; set; }
    }

    public enum TipoUsuario
    {
        Administrador = 1,
        Medico = 2,
        Paciente = 3
    }


}
