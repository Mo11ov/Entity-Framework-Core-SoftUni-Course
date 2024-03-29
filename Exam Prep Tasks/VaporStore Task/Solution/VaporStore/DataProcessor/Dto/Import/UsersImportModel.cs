﻿namespace VaporStore.DataProcessor.Dto.Import
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public class UsersImportModel
    {
        [Required]
        [RegularExpression(@"[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+")]
        public string FullName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Range(3, 103)]
        public int Age { get; set; }

        public ICollection<CardsInputModel> Cards { get; set; }
    }
}
