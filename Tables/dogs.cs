using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kinological_club.Tables;

namespace kinological_club.Tables
{
    [Table("dogs")]
    public class Dogs
    {
        [Key]
        [Column("id_dog")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("nickname")]
        public string Nickname { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Column("birth_date")]
        public DateTime BirthDate { get; set; }

        [Column("death_date")]
        public DateTime? DeathDate { get; set; }

        [Column("gender")]
        public string Gender { get; set; }

        [Column("breed")]
        public string Breed { get; set; }

        [Column("father_id")]
        public int? FatherId { get; set; }

        [Column("mother_id")]
        public int? MotherId { get; set; }

        [Column("exhibition_id")]
        public int? ExhibitionId { get; set; }

        [ForeignKey("OwnerId")]
        public Owners Owner { get; set; }

        [ForeignKey("FatherId")]
        public Dogs Father { get; set; }

        [ForeignKey("MotherId")]
        public Dogs Mother { get; set; }

        [ForeignKey("ExhibitionId")]

        public Exhibition? Exhibition { get; set; }
    }

}
