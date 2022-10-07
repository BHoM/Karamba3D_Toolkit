namespace BH.Engine.Adapters.Karamba3D
{
    using Adapter.Karamba3D;
    using Karamba.CrossSections;
    using Karamba3D_Engine;
    using oM.Structure.SectionProperties;

    public static partial class Convert
    {
        public static ISectionProperty ToBhOM(this CroSec k3dCrossSection)
        {
            if (k3dCrossSection is CroSec_Beam k3dBeamCrossSection)
            {
                return ToBhOM(k3dBeamCrossSection);
            }

            K3dLogger.RecordWarning(string.Format(Resource.WarningNotSupportedType, k3dCrossSection.GetType().Name));
            return null;
        }
    }
}