using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kinological_club.Models
{
    public class AwardsViewModel
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        //[Required(ErrorMessage = "Введите название награды.")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Выберите дату получения награды.")]
        [DataType(DataType.Date)]
        public DateTime? DateOfReceiving { get; set; }
    }
}
