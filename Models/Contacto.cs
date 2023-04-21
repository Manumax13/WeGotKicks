using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeGotKicks.Models
{
     [Table("t_contacto")]
    public class Contacto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //CREAR ID AUTOINCREMENT
        [Column("id")]
        public int Id { get; set; }

        [Column("name")] //como se llama en la tabla
        public string? Name {get;set;}

        [Column("email")]
        public string? Email {get;set;}
        [Column("subject")]
        public string? Subject {get; set; } 
        [Column("comment")]
        public string? Comment {get; set; }
        //? NOS PERMITE Q SEA NULO
    }
}