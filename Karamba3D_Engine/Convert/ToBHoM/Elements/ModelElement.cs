namespace BH.Engine.Adapters.Karamba3D
{
    using System.Linq;
    using Karamba.Elements;
    using Karamba.Models;
    using oM.Dimensional;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IElement ToBhOM(this ModelElement k3dElement, Model k3Model, BhOMModel bhomModel)
        {
            return ToBhOM(k3dElement as dynamic, k3Model, bhomModel);
        }
    }
}