namespace BH.Engine.Adapters.Karamba3D
{
    using Adapter.Karamba3D;
    using BH.oM.Dimensional;
    using BH.oM.Geometry;
    using BH.oM.Structure.Elements;
    using BH.oM.Structure.Offsets;
    using Karamba.Elements;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.SectionProperties;

    public static partial class Convert
    {
        internal static Bar ToBhOM(this ModelElementStraightLine k3dElement, Model k3dModel, BhOMModel bhomModel)
        {
            if (k3dElement is ModelSpring)
            {
                K3dLogger.RecordWarning(string.Format(Resource.WarningNotYetSupportedType, nameof(ModelSpring)));
                return null;
            }
            
            // In Karamba there are 3 eccentricities:
            //- Local beam eccentricity
            //- Global beam eccentricity
            //- Local cross section eccentricity that belongs to the assigned cross section.
            // The offset combines all of them together.
            
            var eccentricityVector = k3dElement.totalEccentricity(k3dModel).ToBhOM();
            var offset = eccentricityVector == new Vector() ?
                null :
                new Offset { Start = eccentricityVector, End = eccentricityVector };
            var release = ((ModelTruss)k3dElement).joint.ToBhOM();
            return new Bar
            {
                Name = k3dElement.ind.ToString(),
                StartNode = bhomModel.Nodes[k3dElement.node_inds[0]],
                EndNode = bhomModel.Nodes[k3dElement.node_inds[1]],
                SectionProperty = (ISectionProperty)k3dElement.crosec.IToBhOM(k3dModel, bhomModel),
                FEAType = k3dElement is ModelBeam ? BarFEAType.Flexural : BarFEAType.Axial,
                Offset = offset,
                OrientationAngle = k3dElement.res_alpha, // TODO check how it works with vertical elements.
                Release = release
            };
        }
    }
}