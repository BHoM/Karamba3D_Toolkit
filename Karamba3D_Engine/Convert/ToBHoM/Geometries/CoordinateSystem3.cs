namespace BH.Engine.Adapters.Karamba3D
{
    using Adapter.Karamba3D;
    using Karamba.Geometry;
    using Karamba3D_Engine;
    using oM.Geometry;
    using oM.Geometry.CoordinateSystem;

    public static partial class Convert
    {
        internal static Cartesian ToBhOM(this CoordinateSystem3 k3dCoSys)
        {
            if (k3dCoSys is null)
            {
                return null;
            }

            return new Cartesian(
                k3dCoSys.Origin.ToBhOM(),
                k3dCoSys.X.ToBhOM(),
                k3dCoSys.Y.ToBhOM(),
                k3dCoSys.Z.ToBhOM());
        }

        internal static Cartesian ToBhOM(this Vector3[] k3dVectorArray)
        {
            if (k3dVectorArray is null)
            {
                return null;
            }

            if (k3dVectorArray.Length != 3)
            {
                K3dLogger.RecordError(
                    string.Format(
                        Resource.ErrorInvalidVectorArray,
                        k3dVectorArray.GetType().GetElementType(),
                        k3dVectorArray.Length));
                return null;
            }
            
            return new Cartesian(
                new Point() { X = 0, Y = 0, Z = 0},
                k3dVectorArray[0].ToBhOM(),
                k3dVectorArray[1].ToBhOM(),
                k3dVectorArray[2].ToBhOM());
        }
    }
}