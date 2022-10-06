namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Geometry;
    using Karamba.Geometry;

    public static partial class Convertt
    {
        public static Point ToBhOM(this Point3 obj)
        {
            Point result = new Point()
            {
                X = obj.X,
                Y = obj.Y,
                Z = obj.Z
            };

            return result;
        }
    }
}