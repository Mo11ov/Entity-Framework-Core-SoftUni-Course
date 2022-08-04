namespace ProductShop
{
    using System;
    using System.Linq;
    using AutoMapper;
    using ProductShop.DTO;
    using ProductShop.Models;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {

            this.CreateMap<UserInputModel, User>();

            this.CreateMap<ProductInputModel, Product>();

            this.CreateMap<CategoryInputModel, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            this.CreateMap<CategoryProductsInputModel, CategoryProduct>();

            this.CreateMap<Product, ExportProductModel>()
                .ForMember(x => x.Seller, y => y.MapFrom(s => s.Seller.FirstName + " " + s.Seller.LastName));

            //this.CreateMap<Category, CategoriesByProductCountModel>()
            //    .ForMember(x => x.ProductsCount, y => y.MapFrom(s => s.CategoryProducts.Count))
            //    .ForMember(x => x.AveragePrice, y => y.MapFrom(s => Math.Round(s.CategoryProducts.Average(z => z.Product.Price), 2)))
            //    .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => Math.Round(s.CategoryProducts.Sum(z => z.Product.Price),2)));

            this.CreateMap<Category, CategoriesByProductCountModel>()
                .ForMember(x => x.ProductsCount, y => y.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(s => s.CategoryProducts.Count == 0 ? 0 : Math.Round(s.CategoryProducts.Average(z => z.Product.Price), 2)))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => Math.Round(s.CategoryProducts.Sum(z => z.Product.Price), 2)));
        }
    }
}
