namespace VaporStore.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;


    [XmlType("User")]
    public class UsersViewModel
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public PurchasViewModel[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotalSpendMoney { get; set; }
    }
}
