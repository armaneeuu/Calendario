using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendario.Models
{
    [Table("t_ambiente")]
    public class Ambiente
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id {get;set;}
        public string? NomAmb {get;set;}
         
    }
}