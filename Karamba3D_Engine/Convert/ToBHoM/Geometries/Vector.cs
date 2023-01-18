namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Geometry;

    public static partial class Convert
    {
        internal static Vector ToBHoM(this Karamba.Geometry.Vector3 obj)
        {
            return new Vector()
            {
                X = obj.X,
                Y = obj.Y,
                Z = obj.Z
            };
        }
    }
}