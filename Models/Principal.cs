using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Protocol.Plugins;

namespace Calendario.Models
{
    [Table("t_principal")]
    public class Principal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Global es requerido")]
        public int GlobalId { get; set; }

        [ForeignKey("GlobalId")]
        public Global Global { get; set; }

        [Required(ErrorMessage = "El campo Específico es requerido")]
        public int EspecificoId { get; set; }

        [ForeignKey("EspecificoId")]
        public Especifico Especifico { get; set; }

        [Required(ErrorMessage = "El campo Responsable Académico es requerido")]
        public int RespAcademicoId { get; set; }

        [ForeignKey("RespAcademicoId")]
        public RespAcademico RespAcademico { get; set; }

        [Required(ErrorMessage = "El campo Responsable Operador es requerido")]
        public int RespOperadorId { get; set; }

        [ForeignKey("RespOperadorId")]
        public RespOperador RespOperador { get; set; }

        public List<AmbienteA>? AmbienteA { get; set; }

        
    }
}