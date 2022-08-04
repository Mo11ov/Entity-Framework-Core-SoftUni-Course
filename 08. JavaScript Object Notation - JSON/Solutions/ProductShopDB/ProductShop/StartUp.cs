using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //Task 01
            //var jsonFilePath = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, jsonFilePath));

            //Task 02
            //var jsonFilePath = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, jsonFilePath));

            //Task 03
            //var jsonFilePath = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, jsonFilePath));

            //Task 04
            //var jsonFilePath = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, jsonFilePath));

            //Task 05
            //Console.WriteLine(GetProductsInRange(context));

            //Task 06
            //Console.WriteLine(GetSoldProducts(context));

            //Task 07
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //Taks 08. Export Users and Products
            Console.WriteLine(GetUsersWithProducts(context));


        }


        //Task 01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson) 
        {
            InitializeMapper();

            var dtoUsers = JsonConvert.DeserializeObject<UserInputModel[]>(inputJson);

            var users = mapper.Map<User[]>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();
            
            return $"Successfully imported {users.Count()}";
        }


        //Task 02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeMapper();

            var dtoProducts = JsonConvert.DeserializeObject<ProductInputModel[]>(inputJson);

            var products = mapper.Map<Product[]>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();
            
            return $"Successfully imported {products.Count()}";
        }


        //Task 03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeMapper();

            var dtoCategories = JsonConvert.DeserializeObject<CategoryInputModel[]>(inputJson)
                .Where(x => x.Name != null)
                .ToArray();

            var categories = mapper.Map<Category[]>(dtoCategories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }


        //Task 04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeMapper();

            var dtoCategoryProduct = JsonConvert.DeserializeObject<CategoryProductsInputModel[]>(inputJson);

            var cateogoryProduct = mapper.Map<CategoryProduct[]>(dtoCategoryProduct);

            context.CategoryProducts.AddRange(cateogoryProduct);
            context.SaveChanges();

            return $"Successfully imported {cateogoryProduct.Count()}";
        }


        //Task 05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeMapper();

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .ProjectTo<ExportProductModel>(mapper.ConfigurationProvider)
                .ToArray();

            var result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }


        //Task 06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.BuyerId.HasValue))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                    .Select(y => new
                    {
                        name = y.Name,
                        price = y.Price,
                        buyerFirstName = y.Buyer.FirstName,
                        buyerLastName = y.Buyer.LastName
                    })
                    .ToArray()
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);

            return result;
        }


        //Task 07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //InitializeMapper();

            //var categories = context.Categories
            //    .ProjectTo<CategoriesByProductCountModel>(mapper.ConfigurationProvider)
            //    .OrderByDescending(x => x.ProductsCount)
            //    .ToArray();

            var categories = context.Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = x.CategoryProducts.Average(y => y.Product.Price).ToString("f2"),
                    totalRevenue = x.CategoryProducts.Sum(y => y.Product.Price).ToString("f2")
                })
                .OrderByDescending(x => x.productsCount)
                .ToArray();

            var result = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return result;
        }


        //Taks 08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToArray()
                .Where(x => x.ProductsSold.Any(y => y.BuyerId.HasValue))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(b => b.BuyerId.HasValue).Count(),
                        products = x.ProductsSold
                        .Where(p => p.BuyerId.HasValue)
                        .Select(y => new
                        {
                            name = y.Name,
                            price = y.Price
                        })
                    }
                })
                .OrderByDescending(x => x.soldProducts.count)
                .ToArray();


            var userWithCount = new
            {
                usersCount = users.Count(),
                users = users
            };
            
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonUsers = JsonConvert.SerializeObject(userWithCount, Formatting.Indented, serializerSettings); 

            return jsonUsers;
        }

        public static void InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}