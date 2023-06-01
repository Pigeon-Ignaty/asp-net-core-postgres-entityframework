using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinological_club.Tables
{
    [Table("exhibition")]
    public class Exhibition
    {
        [Key]
        [Column("id_exhibition")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_dog")]
        public int DogId { get; set; }

        [Column("name_exhibition")]
        public string Name { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }


    }

}
