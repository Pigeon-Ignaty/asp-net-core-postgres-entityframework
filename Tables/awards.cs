using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinological_club.Tables
{
    [Table("awards")]
    public class Award
    {
        [Key]
        [Column("id_award")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_dog")]
        public int DogId { get; set; }

        [Column("name_of_award")]
        public string Name { get; set; }

        [Column("date_of_receiving")]
        public DateTime? DateOfReceiving { get; set; }

        [ForeignKey("DogId")]
        public Dogs Dogs { get; set; }
    }

}
