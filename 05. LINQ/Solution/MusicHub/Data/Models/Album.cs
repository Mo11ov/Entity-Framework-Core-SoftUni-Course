﻿namespace MusicHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Album
    {
        public Album()
        {
            this.Songs = new HashSet<Song>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        [NotMapped]
        public decimal Price
            => this.Songs.Sum(s => s.Price);

        // Navi prop
        public int? ProducerId { get; set; }
        public virtual Producer Producer { get; set; }


        public virtual ICollection<Song> Songs { get; set; }
    }
}
