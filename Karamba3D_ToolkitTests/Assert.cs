namespace Karamba3D_ToolkitTests
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using BH.oM.Base;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public static class CustomAssert
    {
        public static void AllPropertiesAreEqual<T>(T actual, T expected, int counter = 0)
         where T : IBHoMObject
        {
            var allPropertiesExceptGuid = actual
                                          .GetType()
                                          .GetProperties()
                                          .Where(p => p.Name != nameof(actual.BHoM_Guid));

            var stringBuilder = new StringBuilder();
            var pinco = true;
            foreach (var variable in allPropertiesExceptGuid)
            {
                var pActual = variable.GetValue(actual);
                var pExpected = variable.GetValue(expected);

                if (pActual is IEnumerable)
                {
                    // TODO Add try catch
                    var actualEnumerator = ((IEnumerable)pActual).GetEnumerator();
                    var expectedEnumerator = ((IEnumerable)pExpected).GetEnumerator();
                    while (actualEnumerator.MoveNext())
                    {
                        expectedEnumerator.MoveNext();
                        var actualValue = actualEnumerator.Current;
                        var expectedValue = expectedEnumerator.Current;

                        if (actualValue is IBHoMObject)
                        {
                            // TODO check if are both bhom objects
                            AllPropertiesAreEqual((IBHoMObject)actualValue, (IBHoMObject)expectedValue, counter + 1);
                        }
                    }
                }

                if (pActual is IBHoMObject)
                {
                    AllPropertiesAreEqual((IBHoMObject)pActual, (IBHoMObject)pExpected, counter + 1);
                }

                if (!Equals(pActual, pExpected))
                {
                    pinco = false;
                    stringBuilder.AppendLine($"{Enumerable.Repeat("\t", counter)}{variable.Name} are not equal.");
                }
            }

            Assert.IsTrue(pinco, stringBuilder.ToString());
        }

        //public static void AllPropertiesAreAlmostEqual<T>(T actual, T expected)
    }
}