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
            CreateMap<IWant, EconomicCalculator.DTOs.Wants.IWantDTO>()
                .As<EconomicCalculator.DTOs.Wants.WantDTO>();
            CreateMap<EconomicCalculator.DTOs.Wants.IWantDTO, IWant>()
                .As<Want>();
        }
    }
}
