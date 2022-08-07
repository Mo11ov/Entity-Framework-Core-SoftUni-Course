namespace Footballers.DataProcessor.ExportDto
{
    using System.Xml.Serialization;


    [XmlType("Coach")]
    public class CoachViewModel
    {
        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }

        [XmlElement("CoachName")]
        public string CoachName { get; set; }

        [XmlArray("Footballers")]
        public FootballerViewModel[] Footballers { get; set; }
    }
}
