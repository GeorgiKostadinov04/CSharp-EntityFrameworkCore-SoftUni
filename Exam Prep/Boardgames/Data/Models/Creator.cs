
using System.ComponentModel.DataAnnotations;
using Boardgames.Utilities;

namespace Boardgames.Data.Models
{
    public class Creator
    {
        public Creator()
        {
            Boardgames = new HashSet<Boardgame>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Values.creatorFirstNameMaxValue)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(Values.creatorLastNameMaxValue)]
        public string LastName { get; set; }

        public virtual ICollection<Boardgame> Boardgames { get; set; }
    }
}
