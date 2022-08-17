using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        public Truck()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }
        
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        [Required]
        public string VinNumber { get; set; }
        
        public int TankCapacity { get; set; }

        public int CargoCapacity  { get; set; }

        public CategoryType CategoryType { get; set; }

        public MakeType MakeType { get; set; }


        public int DespatcherId  { get; set; }
        public Despatcher Despatcher { get; set; }

        public ICollection<ClientTruck> ClientsTrucks  { get; set; }
    }
}
