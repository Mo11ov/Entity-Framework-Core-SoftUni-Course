﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class TrucksImportModel
    {
        [Required]
        [StringLength(8)]
        [RegularExpression(@"[A-Z]{2}[0-9]{4}[A-Z]{2}")]
        public string RegistrationNumber { get; set; }

        [Required]
        [StringLength(17)]
        public string VinNumber { get; set; }
        
        [Range(950, 1420)]
        public int TankCapacity { get; set; }
        
        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }
        
        public int CategoryType { get; set; }
        
        public int MakeType { get; set; }
    }
}
