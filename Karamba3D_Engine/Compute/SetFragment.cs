using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Modify
    {
        public static void SetIdFragment<T>(this List<object> iElementLoads, List<IBHoMObject> pulledObjects) where T : IBHoMObject
        {
            // Just used for testing. Should modify iElementLoads by ref but needs work.

            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach (IBHoMObject pulledObject in pulledObjects)
            {
                var stringhash = string.Join("", pulledObject.GeometryHash());
                dict.Add(stringhash, pulledObject);
            }

            foreach (object iElementLoad in iElementLoads)
            {
                T loadObject = (iElementLoad as dynamic).Objects.Elements[0];
                var stringhash = string.Join("", loadObject.GeometryHash());

                if (dict.TryGetValue(stringhash, out object pulledObj))
                {
                    loadObject.AddFragment(((IBHoMObject)pulledObj).Fragments.FirstOrDefault());
                }
            }
        }
    }
}
