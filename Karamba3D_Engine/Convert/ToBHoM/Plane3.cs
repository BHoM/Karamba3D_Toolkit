using BH.oM.Geometry;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        public static Basis ToBhOM(this Karamba.Geometry.Plane3 plane)
        {
            return new Basis(plane.XAxis.ToBhOM(), plane.YAxis.ToBhOM(), plane.ZAxis.ToBhOM());

        }
    }
}