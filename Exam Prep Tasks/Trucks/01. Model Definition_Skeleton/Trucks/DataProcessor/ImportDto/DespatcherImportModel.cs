using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
	[XmlType("Despatcher")]
    public class DespatcherImportModel
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; }

        public string Position { get; set; }

		[XmlArray("Trucks")]
        public List<TrucksImportModel> Trucks { get; set; }
    }
}



