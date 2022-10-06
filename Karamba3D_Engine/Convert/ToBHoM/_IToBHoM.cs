namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Base;
    using Karamba.Models;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        // Entry point
        public static IObject IToBhOM(this object obj)
        {
            return ToBhOM(obj as dynamic);
        }

        // Fallback method
        private static IObject ToBhOM(this object obj)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }
    }
}


