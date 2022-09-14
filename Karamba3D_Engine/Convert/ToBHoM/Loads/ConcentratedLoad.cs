namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Elements;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        internal static BarPointLoad ToBhOM<T>(this T k3dLoad, Model k3dModel, BhOMModel bhomModel) where T : ConcentratedLoad
        {

            var elementsFromTags = k3dModel.Elements(k3dLoad.ElementIds);
            var elementsFromGuids = k3dLoad.ElementGuids.SelectMany(k3dModel.Elements);

            var bars = from element in elementsFromTags.Concat(elementsFromGuids)
                       select bhomModel.Elements1D[element.ind];
            var barGroup = new BHoMGroup<Bar>();
            barGroup.Elements.AddRange(bars);
                
            bool isProjected = false;
            LoadAxis loadAxis;
            switch (k3dLoad.LoadOrientation)
            {
                case LoadOrientation.local:
                    loadAxis = LoadAxis.Local;
                    break;

                case LoadOrientation.global:
                    loadAxis = LoadAxis.Global;
                    break;

                case LoadOrientation.proj:
                    loadAxis = LoadAxis.Global;
                    isProjected = true;
                    break;

                default:
                    Base.Compute.RecordError($"{nameof(T)} cannot be converted to {nameof(BarPointLoad)} due to Karamba's {nameof(k3dLoad.LoadOrientation)} property.");
                    return null;
            }

            return new BarPointLoad
            {
                DistanceFromA = k3dLoad.Position,
                Loadcase = bhomModel.RegisterLoadCase(k3dLoad.LcName),
                Objects = barGroup,
                Axis = loadAxis,
                Projected = isProjected
            };
        }
    }
}