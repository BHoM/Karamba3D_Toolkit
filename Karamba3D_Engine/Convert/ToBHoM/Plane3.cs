using BH.oM.Geometry;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        public static Basis ToBHoM(this Karamba.Geometry.Plane3 plane)
        {
            return new Basis(plane.XAxis.ToBHoM(), plane.YAxis.ToBHoM(), plane.ZAxis.ToBHoM());

        }
    }
}