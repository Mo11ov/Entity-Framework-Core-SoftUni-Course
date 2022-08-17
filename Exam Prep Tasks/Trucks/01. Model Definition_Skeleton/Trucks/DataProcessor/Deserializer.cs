namespace Trucks.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";
        //Done
        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<Despatcher> validDespatchers = new List<Despatcher>();

            var dtoDespathcers = XmlConverter.Deserializer<DespatcherImportModel>(xmlString, "Despatchers");

            foreach (var dtoDespatcher in dtoDespathcers)
            {
                List<Truck> validTrucks = new List<Truck>();

                if (!IsValid(dtoDespatcher) || string.IsNullOrEmpty(dtoDespatcher.Position))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Despatcher despatcher = new Despatcher
                {
                    Name = dtoDespatcher.Name,
                    Position = dtoDespatcher.Position
                };

                foreach (var dtoTruck in dtoDespatcher.Trucks)
                {
                    if (!IsValid(dtoTruck))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    Truck truck = new Truck
                    {
                        RegistrationNumber = dtoTruck.RegistrationNumber,
                        VinNumber = dtoTruck.VinNumber,
                        TankCapacity = dtoTruck.TankCapacity,
                        CargoCapacity = dtoTruck.CargoCapacity,
                        CategoryType = (CategoryType)dtoTruck.CategoryType,
                        MakeType = (MakeType)dtoTruck.MakeType
                    };

                    validTrucks.Add(truck);
                }

                despatcher.Trucks = validTrucks;
                validDespatchers.Add(despatcher);

                sb.AppendLine($"Successfully imported despatcher - {despatcher.Name} with {despatcher.Trucks.Count} trucks.");
            }

            context.Despatchers.AddRange(validDespatchers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            List<Client> validClients = new List<Client>();
            StringBuilder sb = new StringBuilder();

            List<int> trucksIdsInDb = context.Trucks.Select(x => x.Id).ToList();

            var dtoClients = JsonConvert.DeserializeObject<ImportClientModel[]>(jsonString);

            foreach (var dtoClient in dtoClients)
            {
                List<ClientTruck> clientTrucks = new List<ClientTruck>();

                if (!IsValid(dtoClient) || dtoClient.Type == "usual")
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Client currClient = new Client 
                {
                    Name = dtoClient.Name,
                    Nationality = dtoClient.Nationality,
                    Type = dtoClient.Type
                };

                foreach (var dtoTruckId in dtoClient.Trucks.Distinct())
                {
                    if (!trucksIdsInDb.Contains(dtoTruckId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    ClientTruck currentClientTruck = new ClientTruck
                    {
                        Client = currClient,
                        TruckId = dtoTruckId
                    };

                    clientTrucks.Add(currentClientTruck);
                }

                currClient.ClientsTrucks = clientTrucks;
                validClients.Add(currClient);

                sb.AppendLine($"Successfully imported client - {currClient.Name} with {currClient.ClientsTrucks.Count} trucks.");
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
