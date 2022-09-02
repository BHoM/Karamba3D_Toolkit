using BH.oM.Geometry;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        public static Vector ToBHoM(this Karamba.Geometry.Vector3 obj)
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