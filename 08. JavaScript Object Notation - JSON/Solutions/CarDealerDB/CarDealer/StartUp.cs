using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();


            ////Task 09. Import Suppliers
            //var jsonSupFilePath = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, jsonSupFilePath));

            ////Taks 10. Import Parts
            //var jsonPartsFilePath = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, jsonPartsFilePath));

            //////Task 11
            //var jsonFilePath = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, jsonFilePath));

            ////Task 12. Import Customers
            //var jsonCustomerFilePath = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, jsonCustomerFilePath));

            //Task 13
            //var jsonSalesPathFile = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, jsonSalesPathFile));

            //Task 14
            //Console.WriteLine(GetOrderedCustomers(context));

            //Task 15
            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //Task 16
            //Console.WriteLine(GetLocalSuppliers(context));

            //Task 17
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //Task 18
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            //Task 19
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        //Task 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeMapper();

            var dtoSuppliers = JsonConvert.DeserializeObject<SuppliersImportModel[]>(inputJson);

            var suppliers = mapper.Map<Supplier[]>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {dtoSuppliers.Count()}.";
        }


        //Taks 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeMapper();

            var dtoParts = JsonConvert.DeserializeObject<PartsImportModel[]>(inputJson);

            var ids = context.Suppliers
                .Select(x => x.Id)
                .ToArray();

            var parts = mapper.Map<Part[]>(dtoParts)
                .Where(x => ids.Contains(x.SupplierId));

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }


        //Taks 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var dtoCars = JsonConvert.DeserializeObject<IEnumerable<CarsInputModel>>(inputJson);

            List<Car> cars = new List<Car>();

            foreach (CarsInputModel dtoCar in dtoCars)
            {
                Car newCar = new Car
                {
                    Make = dtoCar.Make,
                    Model = dtoCar.Model,
                    TravelledDistance = dtoCar.TravelledDistance,
                };
                foreach (int partId in dtoCar.PartsId.Distinct())
                {
                    newCar.PartCars.Add(new PartCar
                    {
                        PartId = partId
                    });
                }

                cars.Add(newCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }


        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeMapper();

            var dtoCustomers = JsonConvert.DeserializeObject<CustomerInputModel[]>(inputJson);

            var customers = mapper.Map<Customer[]>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }


        //Taks 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeMapper();

            var dtoSales = JsonConvert.DeserializeObject<SalesInputModel[]>(inputJson);

            var sales = mapper.Map<Sale[]>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }


        //Taks 14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenByDescending(x => x.IsYoungDriver == false)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }


        //Taks 15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }


        //Task 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }


        //Task 17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars
                    .Select(p => new 
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    })
                })
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }


        //Task 18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count(),
                    spentMoney = x.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }


        //Taks 19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = x.Discount.ToString("f2"),
                    price = x.Car.PartCars.Sum(s => s.Part.Price).ToString("f2"),
                    priceWithDiscount = (x.Car.PartCars.Sum(s => s.Part.Price) - (x.Car.PartCars.Sum(s => s.Part.Price) *(x.Discount / 100))).ToString("f2")
                })
                .ToList();

            var result = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return result;
        }

        public static void InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}