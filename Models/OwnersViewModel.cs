using System.ComponentModel.DataAnnotations.Schema;

namespace kinological_club.Models
{
    public class OwnersViewModel
    {
        public int OwnerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public int DogCount { get; set; }

        public DateTime EarliestBirthDate { get; set; } // Новое свойство
        public DateTime LatestBirthDate { get; set; } // Новое свойство
    }
}
