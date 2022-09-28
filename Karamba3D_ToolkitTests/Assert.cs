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
        public static void BhOMObjectsAreEqual<T>(T actual, T expected, double tolerance = 0.0, string message = "")
        {
            bool areEqual = AreEqualOrPropertiesEqual(actual, expected, tolerance, out var notEqualProperties);

            if (areEqual)
                return;

            var sb = new StringBuilder();
            if (!notEqualProperties.Any())
            {
                sb.AppendLine($"{typeof(T)} are not equal");
            }

            foreach (var property in notEqualProperties)
            {
                sb.AppendLine($"The property \"{property.Name}\" of \"{typeof(T)}\" are not equal");
            }

            message += Environment.NewLine + sb;
            Assert.Fail(message);
        }

        private static bool AreEqualOrPropertiesEqual<T>(T actual, T expected, double tolerance, out IEnumerable<PropertyInfo> notEqualProperties)
        {
            // If the type is string, value type or override the equal method, the equal method will be used
            // else all the public readable properties will be compared
            bool areEqual;
            notEqualProperties = Enumerable.Empty<PropertyInfo>();
            if (EqualityCanBeDirectlyCheck(actual))
            {
                areEqual = Equals(actual, expected);
            }
            else
            {
                areEqual = CheckPropertiesEquality<T>(actual, expected, tolerance, out notEqualProperties);
            }

            return areEqual;
        }

        public static bool OverridesEqualsMethod(object obj)
        {
            return obj.GetType().GetMethods().Any(m => m.Name == "Equals" && m.DeclaringType != typeof(object));
        }

        private static bool CheckPropertiesEquality<T>(T actual, T expected, double tolerance, out IEnumerable<PropertyInfo> notEqualProperties)
        {
            // The comparison will consider all the properties of the type T.
            // 1. If the property type is string, value type or override the equal method,
            // the equal method will be used.
            // 2. If the type is IEnumerable each element of the enumerable will be tested.
            // 3. If the type comes from IObject will be tested for property equality.
            var failProperties = new List<PropertyInfo>();
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
                        // When any entity is not equal, the enumerables are not equal too.
                        areValuesEqual = AreEqualOrPropertiesEqual(actualEnumerator.Current, expectedEnumerator.Current, tolerance, out _);

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
                    areValuesEqual = CheckPropertiesEquality(pActualObject, pExpectedObject, tolerance, out _);
                }
                else
                {
                    throw new ArgumentException($"\"{actualValue.GetType()}\" property cannot be compared.");
                }

                if (!areValuesEqual)
                {
                    failProperties.Add(property);
                }
            }

            notEqualProperties = failProperties;
            return !notEqualProperties.Any();
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

        private static bool AreAlmostEqual(decimal actualValue, decimal expectedValue, double tolerance)
        {
            if (tolerance != 0.0 && 
                (typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(float)))
            {
                return Math.Abs(actualValue - expectedValue) < test;
                
            }
            Equals(actualValue, expectedValue);
        }
    }
}