using kinological_club.Tables;

using System.ComponentModel.DataAnnotations.Schema;

namespace kinological_club.Models
{
    public class DogsViewModel
    {
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
        public IEnumerable<DogsViewModel>? Dogs { get; set; }
        public List<OwnersViewModel>? Owners { get; set; }
        public int DogCount { get; set; }
        public List<BreedStatisticsViewModel>? BreedStatistics { get; set; }
        
    }

    public class BreedStatisticsViewModel
    {
        public string Breed { get; set; }
        public int DogCount { get; set; }
        public DateTime EarliestBirthDate { get; set; } // Новое свойство
        public DateTime LatestBirthDate { get; set; } // Новое свойство
    }

}
