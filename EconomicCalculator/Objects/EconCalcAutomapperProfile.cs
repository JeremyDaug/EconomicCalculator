using AutoMapper;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects
{
    public class EconCalcAutomapperProfile : Profile
    {
        public EconCalcAutomapperProfile()
        {
            CreateMap<Want, EconDTOs.DTOs.Wants.Want>();
            CreateMap<Product, EconDTOs.DTOs.Products.Product>();
        }
    }
}
