using System;
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
                var dataSets = BH.Engine.Library.Query.Datasets(name);
                dataSets.AddRange(BH.Engine.Library.Query.Datasets(string.Concat(name,"+")));

                var number = Regex.Match(obj.name, @"^[0-9]*$");
                obj.name.Replace(number.Value, string.Empty);
                var sectionType = obj.name.Replace(name, string.Empty);

                var allSections = dataSets.SelectMany(d=> d.Data).OfType<ISectionProperty>();

                foreach (var section in allSections)
                {
                    var bhomNumber = Regex.Match(section.Name, @"^[0-9]*$");
                    
                    if (bhomNumber != number)
                        continue;

                    var bhomSectionType = section.Name.Replace(name, string.Empty);
                    bhomSectionType = bhomSectionType.Replace(bhomNumber.Value, string.Empty).Trim();

                    if (bhomSectionType != sectionType)
                        continue;

                    result = section;

                }
            }

            return result;

            
        }
    }
}