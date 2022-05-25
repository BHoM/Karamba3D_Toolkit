using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.oM.Structure.SectionProperties;
using Karamba.CrossSections;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Query
    {
        public static ISectionProperty ToBHoM(this CroSec obj)
        {
            ISectionProperty result = null;
            string name = null;
            if (obj.family.ToUpper().StartsWith("HE"))
            {
                name = "HE";

                var number = Regex.Match(obj.name, @"\d+");
                var sectionType = obj.name.Replace(number.Value, string.Empty);
                sectionType = sectionType.Replace(name, string.Empty);


                List<SteelSection> allSections = Compute.GetDatasetData<SteelSection>(name, true);
                foreach (var section in allSections)
                {
                    var bhomNumber = Regex.Match(section.Name, @"^[0-9]*$");
                    
                    if (bhomNumber != number)
                        continue;

                    var bhomSectionType = section.Name.Replace(name, string.Empty);
                    bhomSectionType = bhomSectionType.Replace(bhomNumber.Value, string.Empty).Trim();

                    if (bhomSectionType != sectionType)
                        continue;

                    result = (ISectionProperty)section;
                }  
            }

            return result;

            
        }
    }
}