namespace BH.Engine.Adapters.Karamba3D
{
    using System.Text.RegularExpressions;

    public static partial class Query
    {
        public static int CrossSectionNumber(this string crossSectionName)
        {
            int sectionNumber = int.Parse(Regex.Match(crossSectionName, @"\d+").Value);
            return sectionNumber;
        }
    }
}


