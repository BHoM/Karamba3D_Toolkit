namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Elements;

    public static partial class Convert
    {
        internal static void GetOrientation<T>(this T k3dLoad, out LoadAxis loadAxis, out bool isProjected) 
            where T : ElementLoad
        {
            isProjected = false;
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
                    var message = string.Format(
                        Resource.ErrorBarPointLoadOrientation,
                        typeof(T),
                        typeof(BarPointLoad),
                        k3dLoad.LoadOrientation.GetType());
                    throw new ArgumentOutOfRangeException(message);
            }
        }

        internal static IEnumerable<Karamba.Elements.ModelElement> GetElements(this ElementLoad k3dLoad, Model k3dModel)
        {

            var elementsFromTags = Enumerable.Empty<ModelElement>();
            var elementsFromGuids = Enumerable.Empty<ModelElement>();
            if (k3dLoad is null || k3dModel is null)
            {
                return Enumerable.Empty<ModelElement>();
            }

            if (k3dLoad.ElementIds?.Any() ?? false)
            {
                elementsFromTags = k3dModel.Elements(k3dLoad.ElementIds);
            }

            if (k3dLoad.ElementGuids?.Any() ?? false)
            {
                elementsFromGuids = k3dLoad.ElementGuids.SelectMany(k3dModel.Elements);
            }

            return elementsFromTags.Concat(elementsFromGuids);
        }

        internal static IEnumerable<int> GetElementIndices(this ElementLoad k3dLoad, Model k3dModel)
        {
            var indicesFromTags = Enumerable.Empty<int>();
            var indicesFromGuids = Enumerable.Empty<int>();

            if (k3dLoad.ElementIds?.Any() ?? false)
            {
                indicesFromTags = k3dModel.ElementInds(k3dLoad.ElementIds);
            }

            if (k3dLoad.ElementGuids?.Any() ?? false)
            {
                indicesFromGuids = k3dModel.ElementInds(k3dLoad.ElementGuids);
            }

            return indicesFromTags.Concat(indicesFromGuids);
        }
    }
}