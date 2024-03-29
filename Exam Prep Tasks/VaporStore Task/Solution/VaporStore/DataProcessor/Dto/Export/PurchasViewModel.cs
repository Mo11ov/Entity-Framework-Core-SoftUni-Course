﻿namespace VaporStore.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;


    [XmlType("Purchase")]
    public class PurchasViewModel
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameViewModel Game { get; set; }
    }
}

