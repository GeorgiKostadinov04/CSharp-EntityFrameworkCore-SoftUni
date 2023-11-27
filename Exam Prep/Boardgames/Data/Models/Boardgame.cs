using Boardgames.Data.Models.Enums;
using Boardgames.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boardgames.Data.Models
{
    public class Boardgame
    {
        public Boardgame()
        {
            BoardgamesSellers = new HashSet<BoardgameSeller>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Values.boardgameNameMaxValue)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Values.boardgameRangeMaxValue)]
        public double Rating { get; set; }

        [Required]
        [MaxLength(Values.boardgameYearMaxValue)]
        public int YearPublished { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        public string Mechanics { get; set; }

        [Required]
        [ForeignKey(nameof(CreatorId))]
        public int CreatorId { get; set; }

        public virtual Creator? Creator { get; set; }

        public virtual ICollection<BoardgameSeller> BoardgamesSellers { get; set; }

    }
}