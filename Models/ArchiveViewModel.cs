using System;
using System.ComponentModel.DataAnnotations;

namespace kinological_club.Models
{
    public class ArchiveViewModel
    {
        [Required(ErrorMessage = "Sas is Required!")]

        public int Id { get; set; }
        public string Nickname { get; set; }
        public int OwnerId { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string Gender { get; set; }
        public string Breed { get; set; }
        public int? FatherId { get; set; }
        public int? MotherId { get; set; }
        public int? ExhibitionId { get; set; }
    }
}
