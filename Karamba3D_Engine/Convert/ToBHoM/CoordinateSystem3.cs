namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using Karamba.Geometry;
    using oM.Geometry;
    using oM.Geometry.CoordinateSystem;

    public static partial class Convert
    {
        public static Cartesian ToBhOM(this CoordinateSystem3 k3dCoSys)
        {
            if (k3dCoSys is null)
            {
                return null;
            }

            return new Cartesian(
                k3dCoSys.Origin.ToBHoM(),
                k3dCoSys.X.ToBHoM(),
                k3dCoSys.Y.ToBHoM(),
                k3dCoSys.Z.ToBHoM());
        }

        public static Cartesian ToBhOM(this Vector3[] k3dVectorArray)
        {
            if (k3dVectorArray is null)
            {
                return null;
            }

            if (k3dVectorArray.Length != 3)
            {
                Base.Compute.RecordError(
                    $"The {k3dVectorArray.GetType().GetElementType()} array should contain 3 elements. Instead it contains {k3dVectorArray.Length} elements");
                return null;
            }

            return new Cartesian(
                Point.Origin,
                k3dVectorArray[0].ToBHoM(),
                k3dVectorArray[1].ToBHoM(),
                k3dVectorArray[2].ToBHoM());
        }
        
    }
}