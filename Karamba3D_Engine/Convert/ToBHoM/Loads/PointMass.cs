namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads;

    public static partial class Convert
    {
        public static void ToBhOM(this PointMass k3dPointMass)
        {
            Base.Compute.RecordError($"{nameof(PointMass)} is not supported yet");

        }
    }
}