using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Technology
{
    public class TechnologyDTO : ITechnologyDTO
    {
        public TechnologyDTO()
        {
            Families = new List<string>();
            FamilyIds = new List<int>();
            Parents = new List<string>();
            ParentIds = new List<int>();
            Children = new List<string>();
            ChildrenIds = new List<int>();
        }

        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public TechCategory CategoryEnum { get; set; }

        public string Category 
        {
            get
            {
                return CategoryEnum.ToString();
            }
            set
            {
                CategoryEnum = (TechCategory)Enum.Parse(typeof(TechCategory), value);
            }
        }

        public int TechBaseCost { get; set; }

        public int Tier { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public List<int> FamilyIds { get; set; }

        public List<string> Families { get; set; }

        [JsonIgnore]
        public string FamilyString
        {
            get
            {
                var result = "";
                foreach (var item in Families)
                {
                    result += item + "; ";
                }
                return result;
            }
        }

        [JsonIgnore]
        public List<int> ChildrenIds { get; set; }

        public List<string> Children { get; set; }

        [JsonIgnore]
        public string ChildrenString
        {
            get
            {
                var result = "";
                foreach (var item in Children)
                {
                    result += item + "; ";
                }
                return result;
            }
        }

        [JsonIgnore]
        public List<int> ParentIds { get; set; }

        public List<string> Parents { get; set; }

        [JsonIgnore]
        public string ParentsString
        {
            get
            {
                var result = "";
                foreach (var item in Parents)
                {
                    result += item + "; ";
                }
                return result;
            }
        }
    
        public void AddTechFamily(TechFamilyDTO fam)
        {
            Families.Add(fam.Name);
            FamilyIds.Add(fam.Id);

            fam.Techs.Add(Id);
            fam.TechStrings.Add(Name);
        }

        public void AddParentTech(TechnologyDTO tech)
        {
            ParentIds.Add(tech.Id);
            Parents.Add(tech.Name);

            // add the reverse.
            tech.ChildrenIds.Add(Id);
            tech.Children.Add(Name);
        }

        public void AddChildTech(TechnologyDTO tech)
        {
            Children.Add(tech.Name);
            ChildrenIds.Add(tech.Id);

            // add teh reverse
            tech.ParentIds.Add(Id);
            tech.Parents.Add(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
