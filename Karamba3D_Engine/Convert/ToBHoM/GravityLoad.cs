namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static GravityLoad ToBhOM(this Karamba.Loads.GravityLoad k3dLoad)
        {
            return new GravityLoad()
            {
                Name = string.Empty,
                Axis = LoadAxis.Global,
                GravityDirection = k3dLoad.force.ToBHoM(),
                Projected = false,
                Loadcase = new Loadcase()
                {
                    Name = k3dLoad.LcName,
                    Number = -1, // TODO assign this
                    Nature = LoadNature.Other,
                }
            };
        }
        
    }
}