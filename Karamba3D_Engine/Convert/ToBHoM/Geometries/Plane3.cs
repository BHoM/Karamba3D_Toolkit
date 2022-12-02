namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Geometry;

    public static partial class Convert
    {
        private static Basis ToBHoM(this Karamba.Geometry.Plane3 plane)
        {
            return new Basis(plane.XAxis.ToBHoM(), plane.YAxis.ToBHoM(), plane.ZAxis.ToBHoM());

        }
    }
}