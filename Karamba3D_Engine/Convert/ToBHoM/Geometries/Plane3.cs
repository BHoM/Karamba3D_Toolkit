namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Geometry;

    public static partial class Convert
    {
        private static Basis ToBhOM(this Karamba.Geometry.Plane3 plane)
        {
            return new Basis(plane.XAxis.ToBhOM(), plane.YAxis.ToBhOM(), plane.ZAxis.ToBhOM());

        }
    }
}