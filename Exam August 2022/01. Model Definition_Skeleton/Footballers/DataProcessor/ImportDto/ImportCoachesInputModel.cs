using Footballers.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class ImportCoachesInputModel
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("Nationality")]
        public string Nationality { get; set; }

        [XmlArray("Footballers")]
        public FootBallerInputModel[] Footballers { get; set; }
    }

    [XmlType("Footballer")]
    public class FootBallerInputModel
    {
        [XmlElement("Name")]
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; }

        [XmlElement("ContractStartDate")]
        [Required]
        public string ContractStartDate { get; set; }

        
        [XmlElement("ContractEndDate")]
        [Required]
        public string ContractEndDate { get; set; }

        [EnumDataType(typeof(BestSkillType))]
        [XmlElement("BestSkillType")]
        public int BestSkillType { get; set; }

        [EnumDataType(typeof(PositionType))]
        [XmlElement("PositionType")]
        public int PositionType { get; set; }
    }
}
