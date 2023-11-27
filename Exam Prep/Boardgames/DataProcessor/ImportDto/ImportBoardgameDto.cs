using Boardgames.Data.Models.Enums;
using Boardgames.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportBoardgameDto
    {
        [Required]
        [MinLength(Values.boardgameNameMinValue)]
        [MaxLength(Values.boardgameNameMaxValue)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [Range(Values.boardgameRangeMinValue, Values.boardgameRangeMaxValue)]
        [XmlElement("Rating")]
        public double Rating { get; set; }

        [Required]
        [Range(Values.boardgameYearMinValue, Values.boardgameYearMaxValue)]
        [XmlElement("YearPublished")]

        public int YearPublished { get; set; }

        [Required]
        [XmlElement("CategoryType")]
        [Range(0,4)]
        public int CategoryType { get; set; }

        [Required]
        [XmlElement("Mechanics")]
        public string Mechanics { get; set; }
    }
}