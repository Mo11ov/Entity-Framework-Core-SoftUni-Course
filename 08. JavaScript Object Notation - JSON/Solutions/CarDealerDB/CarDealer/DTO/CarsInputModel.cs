﻿namespace CarDealer.DTO
{
    using System.Collections.Generic;

    public class CarsInputModel
    {
        public string Make { get; set; }


        public string Model { get; set; }


        public long TravelledDistance { get; set; }


        public IEnumerable<int> PartsId { get; set; }
    }
}
