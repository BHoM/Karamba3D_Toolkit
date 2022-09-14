namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads.Beam;

    public static partial class Convert
    {
        public static void ToBhOM(this RotationalGap k3dLoad)
        {
            Base.Compute.RecordError(
                string.Format(
                    Resource.NotSupportedTypeError,
                    typeof(RotationalGap)));
        }
        
    }
}