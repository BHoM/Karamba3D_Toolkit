namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using Adapter.Karamba3D;
    using BH.oM.Base;
    using Karamba.Materials;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static ISet<Type> UnsupportedType = new HashSet<Type>()
        {
            typeof(FemMaterial_Orthotropic),
        };

        internal static object IToBHoM(this object obj, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            return ToBHoM(obj as dynamic, k3dModel, bhomModel);
        }

        // Fallback methods
        private static IObject ToBHoM(this object obj)
        {
            K3dLogger.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }

        private static IObject ToBHoM(this object obj, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            if (UnsupportedType.Contains(obj.GetType()))
            {
                K3dLogger.RecordWarning(string.Format(Resource.WarningNotYetSupportedType, obj.GetType().FullName));
            }
            else
            {
                K3dLogger.RecordError(string.Format(Resource.ErrorConverterNotFound, obj.GetType().FullName));
            }
            return null;
        }
    }
}


