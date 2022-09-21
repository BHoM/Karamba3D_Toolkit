﻿namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Structure.Loads;

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

            var elementsFromTags = k3dModel.Elements(k3dLoad.ElementIds);
            var elementsFromGuids = k3dLoad.ElementGuids.SelectMany(k3dModel.Elements);

            return elementsFromTags.Concat(elementsFromGuids);
        }

        internal static IEnumerable<int> GetElementIndices(this ElementLoad k3dLoad, Model k3dModel)
        {
            var indicesFromTags = k3dModel.ElementInds(k3dLoad.ElementIds);
            var indicesFromGuids = k3dModel.ElementInds(k3dLoad.ElementGuids);

            return indicesFromTags.Concat(indicesFromGuids);
        }
    }
}