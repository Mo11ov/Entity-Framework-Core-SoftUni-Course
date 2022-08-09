namespace VaporStore.DataProcessor.Dto.Import
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;


    public class CardsInputModel
    {
        [Required]
        [RegularExpression(@"([0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4})")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{3}")]
        public string CVC { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
