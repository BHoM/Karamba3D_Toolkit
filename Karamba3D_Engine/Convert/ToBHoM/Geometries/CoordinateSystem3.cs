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
                k3dCoSys.Origin.ToBhOM(),
                k3dCoSys.X.ToBhOM(),
                k3dCoSys.Y.ToBhOM(),
                k3dCoSys.Z.ToBhOM());
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
                    string.Format(
                        Resource.InvalidVectorArrayError,
                        k3dVectorArray.GetType().GetElementType(),
                        k3dVectorArray.Length));
                return null;
            }

            return new Cartesian(
                Point.Origin,
                k3dVectorArray[0].ToBhOM(),
                k3dVectorArray[1].ToBhOM(),
                k3dVectorArray[2].ToBhOM());
        }
        
    }
}