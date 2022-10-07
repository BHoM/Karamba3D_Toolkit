namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Elements;
    using Karamba.Models;
    using oM.Dimensional;

    public static partial class Convert
    {
        private static IElement IToBhOM(this ModelElement k3dElement, Model k3Model, BhOMModel bhomModel)
        {
            return ToBhOM(k3dElement as dynamic, k3Model, bhomModel);
        }
    }
}