namespace Trucks.DataProcessor
{
    using System;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using Trucks.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despathcers = context.Despatchers
                .Where(x => x.Trucks.Any())
                .ToList()
                .Select(x => new DespatchersViewModel
                {
                    DespatcherName = x.Name,
                    TrucksCount = x.Trucks.Count,
                    Trucks = x.Trucks.Select(t => new TruckViewModel
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    })
                    .OrderBy(x => x.RegistrationNumber)
                    .ToList()
                })
                .OrderByDescending(x => x.TrucksCount)
                .ThenBy(x => x.DespatcherName)
                .ToList();

            var result = XmlConverter.Serialize(despathcers, "Despatchers");

            return result;
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(x => x.ClientsTrucks.Any(t => t.Truck.TankCapacity >= capacity))
                .ToList()
                .Select(x => new
                {
                    Name = x.Name,
                    Trucks = x.ClientsTrucks.Select(t => new
                    {
                        TruckRegistrationNumber = t.Truck.RegistrationNumber,
                        VinNumber = t.Truck.VinNumber,
                        TankCapacity = t.Truck.TankCapacity,
                        CargoCapacity = t.Truck.CargoCapacity,
                        CategoryType = t.Truck.CategoryType.ToString(),
                        MakeType = t.Truck.MakeType.ToString()
                    })
                    .Where(x => x.TankCapacity >= capacity)
                    .OrderBy(x => x.MakeType)
                    .ThenByDescending(x => x.CargoCapacity)
                    .ToList()
                })
                .OrderByDescending(x => x.Trucks.Count)
                .ThenBy(x => x.Name)
                .Take(10)
                .ToList();

            var result = JsonConvert.SerializeObject(clients, Formatting.Indented);

            return result;
        }
    }
}
