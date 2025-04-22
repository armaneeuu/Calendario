using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendario.Models
{
    [Table("t_ambientea")]
    public class AmbienteA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El campo Ambiente es requerido")]
        public int AmbienteId { get; set; }

        [ForeignKey("AmbienteId")]
        public Ambiente Ambiente { get; set; }

        public int PrincipalId { get; set; }

        [ForeignKey("PrincipalId")]
        public Principal? Principal { get; set; }

        public string? Codigo {get; set;}
    }

}