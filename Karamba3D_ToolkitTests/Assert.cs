namespace Karamba3D_ToolkitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing.Design;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using BH.oM.Base;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public static class CustomAssert
    {
        public static void AllPropertiesAreEqual<T>(T actual, T expected)
        {
            CustomEquality(actual, expected, 0);
        }

        private static bool CustomEquality<T>(T actual, T expected, int loopLevel = 0)
        {
            // If the type is string, value type or override the equal method, the equal method will be used
            // else all the public readable properties will be compared
            var stringBuilder = new StringBuilder();
            bool arePropertiesEqual = true;
            if (EqualityCanBeDirectlyCheck(actual))
            {
                arePropertiesEqual = Equals(actual, expected);
            }
            else
            {
                arePropertiesEqual = CheckPropertyEquality<T>(actual, expected);
            }

            if (loopLevel == 0 && !arePropertiesEqual)
            {
                Assert.Fail(stringBuilder.ToString());
            }
            return arePropertiesEqual;
        }

        public static bool OverridesEqualsMethod(object obj)
        {
            return obj.GetType().GetMethods().Any(m => m.Name == "Equals" && m.DeclaringType != typeof(object));
        }

        private static bool CheckPropertyEquality<T>(T actual, T expected, int loopLevel = 0)
        {
            foreach (var property in GetAllPublicReadableProperties(actual))
            {
                var areValuesEqual = true;
                var actualValue = property.GetValue(actual);
                var expectedValue = property.GetValue(expected);

                if (EqualityCanBeDirectlyCheck(actualValue))
                {
                    areValuesEqual = Equals(actualValue, expectedValue);
                }
                else if (actualValue is IEnumerable actualEnumerable)
                {
                    var expectedEnumerable = expectedValue as IEnumerable;
                    var actualEnumerator = actualEnumerable.GetEnumerator();
                    var expectedEnumerator = expectedEnumerable?.GetEnumerator();

                    bool actualCanMove = actualEnumerator.MoveNext();
                    bool expectedCanMove = expectedEnumerator?.MoveNext() ?? false;
                    while (actualCanMove && expectedCanMove && areValuesEqual)
                    {
                        // When any entity is not property equal, the enumerables are not equal.
                        areValuesEqual = CustomEquality(actualEnumerator.Current, expectedEnumerator.Current, loopLevel + 1);

                        actualCanMove = actualEnumerator.MoveNext();
                        expectedCanMove = expectedEnumerator.MoveNext();
                    }

                    // If one can move and the other no, the enumerables
                    // have different length and are not equal.
                    if (actualCanMove || expectedCanMove)
                    {
                        areValuesEqual = false;
                    }

                    if (actualEnumerator is IDisposable actualDisposable)
                    {
                        actualDisposable.Dispose();
                    }

                    if (expectedEnumerator is IDisposable expectedDisposable)
                    {
                        expectedDisposable.Dispose();
                    }
                }
                else if (actualValue is IObject pActualObject)
                {
                    var pExpectedObject = expectedValue as IObject;
                    areValuesEqual = CustomEquality(pActualObject, pExpectedObject, loopLevel + 1);
                }
                else
                {
                    throw new ArgumentException($"{actualValue.GetType()} cannot be compared");
                }

                if (loopLevel == 0 && !areValuesEqual)
                {
                    stringBuilder.AppendLine($"{typeof(T)}.{property.Name} are not equal.");
                }

                arePropertiesEqual &= areValuesEqual;
            }
        }

        private static IEnumerable<PropertyInfo> GetAllPublicReadableProperties(object obj)
        {
            return obj.GetType()
                      .GetProperties()
                      .Where(p => p.Name != "BHoM_Guid")
                      .Where(p => p.CanRead)
                      .Where(p => p.GetMethod?.IsPublic ?? false);
        }

        private static bool EqualityCanBeDirectlyCheck(object obj)
        {
            return obj is null || obj.GetType().IsValueType || obj is string || OverridesEqualsMethod(obj);
        }
    }
}