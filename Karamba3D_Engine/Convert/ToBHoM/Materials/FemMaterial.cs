namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Structure.MaterialFragments;
    using Karamba.Materials;
    using System;

    public static partial class Convert
    {
        /***************************************************/
        /*** Methods                                     ***/
        /***************************************************/

        public static IMaterialFragment ToBhOM(this Karamba.Materials.FemMaterial k3dMaterial)
        {
            if (k3dMaterial is FemMaterial_Isotrop k3dIsotropic)
                return k3dIsotropic.ToBhOM();

            throw new NotImplementedException($"Material conversion for `{k3dMaterial.name}` not yet implemented.");
        }


        /***************************************************/
    }
}


