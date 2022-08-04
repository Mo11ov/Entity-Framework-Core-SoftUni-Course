using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SuppliersImportModel, Supplier>();

            this.CreateMap<PartsImportModel, Part>();

            this.CreateMap<CustomerInputModel, Customer>();

            this.CreateMap<SalesInputModel, Sale>();
        }
    }
}
