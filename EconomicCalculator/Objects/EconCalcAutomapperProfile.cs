using AutoMapper;
using EconomicCalculator.DTOs.Products;
using EconomicCalculator.DTOs.Technology;
using EconomicCalculator.DTOs.Wants;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Products.ProductTags;
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
            CreateMap<Want, WantDTO>();

            CreateMap<Technology.Technology, TechnologyDTO>()
                .ForMember(dest => dest.FamilyIds,
                           act => act.MapFrom(src => src.Families.Select(x => x.Id)))
                .ForMember(dest => dest.Families,
                           act => act.MapFrom(src => src.Families.Select(x => x.Name)))

                .ForMember(dest => dest.Children,
                           act => act.MapFrom(src => src.Children.Select(x => x.Name)))
                .ForMember(dest => dest.ChildrenIds,
                           act => act.MapFrom(src => src.Children.Select(x => x.Id)))
                
                .ForMember(dest => dest.Parents,
                           act => act.MapFrom(src => src.Children.Select(x => x.Name)))
                .ForMember(dest => dest.ParentIds,
                           act => act.MapFrom(src => src.Children.Select(x => x.Id)));

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Name,
                           act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.VariantName,
                           act => act.MapFrom(src => src.VariantName))
                .ForMember(dest => dest.Wants,
                           act => act.MapFrom(src => ProductWantsToProductDTOWants(src.Wants)))
                .ForMember(dest => dest.WantStrings,
                           act => act.MapFrom(src => 
                               new List<string>(src.Wants
                                   .Select(x => x.Item1.Name + "<" + x.Item2.ToString() + ">"))))
                .ForMember(dest => dest.TagStrings,
                           act => act.MapFrom(src => ProductTagToDTOString(src.ProductTags)));
        }

        private Dictionary<int, decimal> ProductWantsToProductDTOWants(IReadOnlyList<(IWant want, decimal amount)> wants)
        {
            var result = new Dictionary<int, decimal>();

            foreach (var pair in wants)
            {
                result[pair.want.Id] = pair.amount;
            }

            return result;
        }

        private List<string> ProductTagToDTOString(IReadOnlyDictionary<ProductTag, Dictionary<string, object>> tags)
        {
            var result = new List<string>();

            foreach (var tag in tags)
            {
                var tempTag = tag.Key.ToString();
                // if tag params greater than 0, add those too.
                if (tag.Value != null)
                {
                    tempTag += "<";
                    foreach (var item in tag.Value)
                    {
                        tempTag += item.ToString() + ";";
                    }
                    // remove trailing ; and close >
                    tempTag = tempTag.TrimEnd(';') + ">";

                }
                result.Add(tempTag);
            }
            return result;
        }
    }
}
