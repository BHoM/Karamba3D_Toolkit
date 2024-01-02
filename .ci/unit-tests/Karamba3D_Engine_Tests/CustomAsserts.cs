/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Karamba3D_Engine_Tests
{
    public static class CustomAsserts
    {
        public static void BhOMObjectsAreEqual<T>(T actual, T expected, EqualityTestOptions options = default)
        {
            if (options is null)
            {
                options = new EqualityTestOptions();
            }
            bool areEqual = AreEqualOrPropertiesEqual(actual, expected, out var notEqualProperties, options);

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

            string message = string.Empty;
            if (options.FailureMessage != string.Empty)
            {
                message += options.FailureMessage + Environment.NewLine;
            }
            Assert.Fail(message + sb);
        }

        private static bool AreEqualOrPropertiesEqual<T>(T actual, T expected, out IEnumerable<PropertyInfo> notEqualProperties, EqualityTestOptions options)
        {
            // If the type is string, value type or override the equal method, the equal method will be used
            // else all the public readable properties will be compared
            bool areEqual;
            notEqualProperties = Enumerable.Empty<PropertyInfo>();

            if (actual == null || expected == null)
            {
                return actual == null && expected == null;
            }
            
            if (EqualityCanBeDirectlyCheck(actual) && !options.AreTolerancesEnabled)
            {
                areEqual = Equals(actual, expected);
            }
            else
            {
                areEqual = CheckPropertiesEquality(actual, expected, out notEqualProperties, options);
            }

            return areEqual;
        }

        public static bool OverridesEqualsMethod(object obj)
        {
            return obj.GetType().GetMethods().Any(m => m.Name == "Equals" && m.DeclaringType != typeof(object));
        }

        private static bool CheckPropertiesEquality<T>(T actual, T expected, out IEnumerable<PropertyInfo> notEqualProperties, EqualityTestOptions options)
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
                var actualValue = property?.GetValue(actual);
                var expectedValue = property?.GetValue(expected);

                if (actualValue == null || expectedValue == null)
                {
                    areValuesEqual = actualValue == null && expectedValue == null;
                }
                else if (EqualityCanBeDirectlyCheck(actualValue))
                {
                    areValuesEqual = AreAlmostEqual(actualValue, expectedValue, options);
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
                        areValuesEqual = AreEqualOrPropertiesEqual(actualEnumerator.Current, expectedEnumerator.Current, out _, options);

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
                    areValuesEqual = CheckPropertiesEquality(pActualObject, pExpectedObject, out _, options);
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
            return obj.GetType().IsValueType || obj is string || OverridesEqualsMethod(obj);
        }

        private static bool AreAlmostEqual<T>(T actualValue, T expectedValue, EqualityTestOptions options)
        {
            if (!options.AreTolerancesEnabled)
                return Equals(actualValue, expectedValue);

            if (actualValue is double actualDouble && 
                expectedValue is double expectedDouble)
            {
                return Math.Abs(actualDouble - expectedDouble) < options.DoubleTolerance;
            }

            if (actualValue is float actualFloat && 
                expectedValue is float expectedFloat)
            {
                return Math.Abs(actualFloat - expectedFloat) < options.SingleTolerance;
            }

            if (actualValue is decimal actualDecimal && 
                expectedValue is decimal expectedDecimal)
            {
                return Math.Abs(actualDecimal - expectedDecimal) < options.DecimalTolerance;
            }

            if (actualValue is IObject)
            {
                return CheckPropertiesEquality(actualValue, expectedValue, out _, options);
            }

            return Equals(actualValue, expectedValue);
        }
    }
}
