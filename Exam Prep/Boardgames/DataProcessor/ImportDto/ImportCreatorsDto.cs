using Boardgames.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorsDto
    {
        [Required]
        [MaxLength(Values.creatorFirstNameMaxValue)]
        [MinLength(Values.creatorFirstNameMinValue)]
        [XmlElement("FirstName")]
        public string FirstName {  get; set; }

        [Required]
        [MaxLength(Values.creatorLastNameMaxValue)]
        [MinLength(Values.creatorLastNameMinValue)]
        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlArray("Boardgames")]
        public virtual ImportBoardgameDto[] Boardgames { get; set; }
    }
}
