using System.Collections.Concurrent;
using System.Reflection;

namespace EXLRT.Xperience.Schemas.Helpers
{
    internal static class ReflectionHelper
    {
        internal static T GetOptimizedPropertyValue<T>(this ReflectionPropertiesHelper[] properties, object obj, string propName) where T : class
        {
            return properties.FirstOrDefault(p => p.Name.Equals(propName))?.Getter(obj) as T;
        }

    }

    // Reflection Optimization
    // https://www.automatetheplanet.com/optimize-csharp-reflection-using-delegates/
    internal class ReflectionPropertiesHelper
    {
        private static readonly ConcurrentDictionary<Type, ReflectionPropertiesHelper[]> Cache = new();
        private static readonly MethodInfo CallInnerDelegateMethod =
            typeof(ReflectionPropertiesHelper).GetMethod(nameof(CallInnerDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        internal string Name { get; set; }

        internal Func<object, object> Getter { get; set; }

        internal static ReflectionPropertiesHelper[] GetProperties(Type type)
            => Cache
                .GetOrAdd(type, _ => type
                .GetProperties()
                .Select(property =>
                {
                    var getMethod = property.GetMethod;
                    var declaringClass = property.DeclaringType;
                    var typeOfResult = property.PropertyType;
                    var getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);
                    var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);
                    var callInnerGenericMethodWithTypes = CallInnerDelegateMethod
                        .MakeGenericMethod(declaringClass, typeOfResult);
                    var result = (Func<object, object>)callInnerGenericMethodWithTypes.Invoke(null, new[] { getMethodDelegate });
                    return new ReflectionPropertiesHelper
                    {
                        Name = property.Name,
                        Getter = result
                    };
                })
                .ToArray());

        private static Func<object, object> CallInnerDelegate<TClass, TResult>(Func<TClass, TResult> deleg)
            => instance => deleg((TClass)instance);
    }
}
