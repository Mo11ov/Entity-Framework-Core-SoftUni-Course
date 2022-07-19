namespace P03_FootballBetting.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Team
    {
        public Team()
        {
            this.HomeGames = new HashSet<Game>();
            this.AwayGames = new HashSet<Game>();
            this.Players = new HashSet<Player>();
        }

        public int TeamId { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string Initials { get; set; }

        public decimal Budget { get; set; }

        //Navigational properties
        public int PrimaryKitColorId { get; set; }
        [InverseProperty(nameof(Color.PrimaryKitTeams))]
        public Color PrimaryKitColor { get; set; }


        public int SecondaryKitColorId { get; set; }
        [InverseProperty(nameof(Color.SecondaryKitTeams))]
        public Color SecondaryKitColor { get; set; }


        public int TownId { get; set; }
        public Town Town { get; set; }


        public ICollection<Game> HomeGames { get; set; }

        public ICollection<Game> AwayGames { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
