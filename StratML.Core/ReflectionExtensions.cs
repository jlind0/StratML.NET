using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections;
using System.Threading;

namespace StratML.Core
{
    public static class ReflectionExtensions
    {
        public static TResult CreateRelatedInstance<TResult>(this object obj)
        {
            TResult result = Activator.CreateInstance<TResult>();
            result.ApplyProperties(obj);
            return result;
        }

        public static object CreateRelatedInstance(this object obj, Type newType)
        {
            var result = Activator.CreateInstance(newType);
            result.ApplyProperties(obj);
            return result;
        }

        public static void ApplyProperties(this object target, object source)
        {
            var targetType = target.GetType();
            foreach (var property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                var targetProperty = targetType.GetProperty(property.Name);
                if (targetProperty == null || targetProperty.GetSetMethod() == null)
                    continue;

                var value = property.GetValue(source);

                if (value != null)
                {
                    if (targetProperty.PropertyType != property.PropertyType)
                    {
                        var collection = value as IEnumerable;
                        if (collection != null && targetProperty.PropertyType.HasInterface(typeof(IList)))
                        {
                            IList targetCollection = Activator.CreateInstance(targetProperty.PropertyType) as IList;
                            Type targetCollectionElementType = targetProperty.PropertyType.GetCollectionValueType();
                            foreach (var val in collection)
                            {
                                var v = val.CreateRelatedInstance(targetCollectionElementType);
                                targetCollection.Add(v);
                            }
                        }

                        value = value.CreateRelatedInstance(targetProperty.PropertyType);
                    }

                    targetProperty.SetValue(target, value);
                }
            }
        }

        private static readonly Lazy<ConcurrentDictionary<Type, Type>> CollectionValueTypeMappingLazy = new Lazy<ConcurrentDictionary<Type, Type>>(() => new ConcurrentDictionary<Type, Type>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Type, Type> CollectionValueTypeMapping
        {
            get { return CollectionValueTypeMappingLazy.Value; }
        }

        public static Type GetCollectionValueType(this Type collectionType)
        {
            Type collectionTypeValue;
            if (!CollectionValueTypeMapping.TryGetValue(collectionType, out collectionTypeValue))
            {
                if (collectionType.IsArray)
                    collectionTypeValue = collectionType.GetElementType();
                else
                    collectionTypeValue = collectionType.GetInterfacesCached().Where(i => i.IsGenericType == true && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(
                            i => i.GetGenericArguments()[0]).Single();
                CollectionValueTypeMapping.TryAdd(collectionType, collectionTypeValue);
            }
            return collectionTypeValue;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, Type>, Type>> GenericInterfaceMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, Type>, Type>>(() => new ConcurrentDictionary<Tuple<Type, Type>, Type>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<Type, Type>, Type> GenericInterfaceMapping
        {
            get { return GenericInterfaceMappingLazy.Value; }
        }

        public static Type GetGenericInterface(this Type target, Type interfaceType)
        {
            Type genericInterface;
            var key = new Tuple<Type, Type>(target, interfaceType);
            if (!GenericInterfaceMapping.TryGetValue(key, out genericInterface))
            {
                genericInterface = target.GetInterfacesCached().Where(i => i.IsGenericType == true && i.GetGenericTypeDefinition() == interfaceType).FirstOrDefault();
                GenericInterfaceMapping.TryAdd(key, genericInterface);
            }
            return genericInterface;
        }

        private static readonly Lazy<ConcurrentDictionary<Type, Type[]>> InterfacesCachedMappingLazy = new Lazy<ConcurrentDictionary<Type, Type[]>>(() => new ConcurrentDictionary<Type, Type[]>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Type, Type[]> InterfacesCachedMapping
        {
            get { return InterfacesCachedMappingLazy.Value; }
        }

        public static Type[] GetInterfacesCached(this Type type)
        {
            Type[] interfaces;
            if (!InterfacesCachedMapping.TryGetValue(type, out interfaces))
            {
                interfaces = type.GetInterfaces();
                InterfacesCachedMapping.TryAdd(type, interfaces);
            }
            return interfaces;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, Type>, Type>> GenericInterfacesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, Type>, Type>>(() => new ConcurrentDictionary<Tuple<Type, Type>, Type>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Tuple<Type, Type>, Type> GenericInterfacesCachedMapping
        {
            get { return GenericInterfacesCachedMappingLazy.Value; }
        }

        public static Type GetGenericInterfaceCached(this Type type, Type genericInterfaceType)
        {
            Type genericInterface;
            var key = new Tuple<Type, Type>(type, genericInterfaceType);
            if (!GenericInterfacesCachedMapping.TryGetValue(key, out genericInterface))
            {
                genericInterface = type.GetGenericInterface(genericInterfaceType);
                GenericInterfacesCachedMapping.TryAdd(key, genericInterface);
            }
            return genericInterface;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, BindingFlags>, PropertyInfo[]>> PropertiesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, BindingFlags>, PropertyInfo[]>>(() => new ConcurrentDictionary<Tuple<Type, BindingFlags>, PropertyInfo[]>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<Type, BindingFlags>, PropertyInfo[]> PropertiesCachedMapping
        {
            get { return PropertiesCachedMappingLazy.Value; }
        }

        public static PropertyInfo[] GetPropertiesCached(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
        {
            PropertyInfo[] props;
            var key = new Tuple<Type, BindingFlags>(type, flags);
            if (!PropertiesCachedMapping.TryGetValue(key, out props))
            {
                props = type.GetProperties(flags);
                PropertiesCachedMapping.TryAdd(key, props);
            }
            return props;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, BindingFlags>, MethodInfo[]>> MethodsCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, BindingFlags>, MethodInfo[]>>(() => new ConcurrentDictionary<Tuple<Type, BindingFlags>, MethodInfo[]>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<Type, BindingFlags>, MethodInfo[]> MethodsCachedMapping
        {
            get { return MethodsCachedMappingLazy.Value; }
        }

        public static MethodInfo[] GetMethodsCached(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
        {
            MethodInfo[] props;
            var key = new Tuple<Type, BindingFlags>(type, flags);
            if (!MethodsCachedMapping.TryGetValue(key, out props))
            {
                props = type.GetMethods(flags);
                MethodsCachedMapping.TryAdd(key, props);
            }
            return props;
        }

        private static readonly Lazy<ConcurrentDictionary<MethodInfo, ParameterInfo[]>> ParametersCachedMappingLazy = new Lazy<ConcurrentDictionary<MethodInfo, ParameterInfo[]>>(() => new ConcurrentDictionary<MethodInfo, ParameterInfo[]>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<MethodInfo, ParameterInfo[]> ParametersCachedMapping
        {
            get { return ParametersCachedMappingLazy.Value; }
        }

        public static ParameterInfo[] GetParametersCached(this MethodInfo method)
        {
            ParameterInfo[] props;
            if (!ParametersCachedMapping.TryGetValue(method, out props))
            {
                props = method.GetParameters();
                ParametersCachedMapping.TryAdd(method, props);
            }
            return props;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, Type, bool>, Attribute>> CustomAttributeCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, Type, bool>, Attribute>>(() => new ConcurrentDictionary<Tuple<Type, Type, bool>, Attribute>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<Type, Type, bool>, Attribute> CustomAttributeCachedMapping
        {
            get { return CustomAttributeCachedMappingLazy.Value; }
        }

        public static T GetCustomAttributeCached<T>(this Type type, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            Attribute attr;
            var key = new Tuple<Type, Type, bool>(type, attrType, inherit);
            if (!CustomAttributeCachedMapping.TryGetValue(key, out attr))
            {
                attr = type.GetCustomAttribute<T>(inherit);
                CustomAttributeCachedMapping.TryAdd(key, attr);
            }
            return attr as T;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, Type, bool>, IEnumerable<Attribute>>> CustomAttributesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, Type, bool>, IEnumerable<Attribute>>>(() => new ConcurrentDictionary<Tuple<Type, Type, bool>, IEnumerable<Attribute>>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<Type, Type, bool>, IEnumerable<Attribute>> CustomAttributesCachedMapping
        {
            get { return CustomAttributesCachedMappingLazy.Value; }
        }

        public static IEnumerable<T> GetCustomAttributesCached<T>(this Type type, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            IEnumerable<Attribute> attr;
            var key = new Tuple<Type, Type, bool>(type, attrType, inherit);
            if (!CustomAttributesCachedMapping.TryGetValue(key, out attr))
            {
                attr = type.GetCustomAttributes<T>(inherit);
                CustomAttributesCachedMapping.TryAdd(key, attr);
            }
            return attr.Cast<T>();
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, Attribute>> ParameterAttributeCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, Attribute>>(() => new ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, Attribute>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, Attribute> ParameterAttributeCachedMapping
        {
            get { return ParameterAttributeCachedMappingLazy.Value; }
        }

        public static T GetCustomAttributeCached<T>(this ParameterInfo parameter, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            Attribute attr;
            var key = new Tuple<ParameterInfo, Type, bool>(parameter, attrType, inherit);
            if (!ParameterAttributeCachedMapping.TryGetValue(key, out attr))
            {
                attr = parameter.GetCustomAttribute<T>(inherit);
                ParameterAttributeCachedMapping.TryAdd(key, attr);
            }
            return attr as T;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, IEnumerable<Attribute>>> ParameterCustomAttributesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, IEnumerable<Attribute>>>(() => new ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, IEnumerable<Attribute>>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Tuple<ParameterInfo, Type, bool>, IEnumerable<Attribute>> ParameterCustomAttributesCachedMapping
        {
            get { return ParameterCustomAttributesCachedMappingLazy.Value; }
        }

        public static IEnumerable<T> GetCustomAttributesCached<T>(this ParameterInfo parameter, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            IEnumerable<Attribute> attr;
            var key = new Tuple<ParameterInfo, Type, bool>(parameter, attrType, inherit);
            if (!ParameterCustomAttributesCachedMapping.TryGetValue(key, out attr))
            {
                attr = parameter.GetCustomAttributes<T>(inherit);
                ParameterCustomAttributesCachedMapping.TryAdd(key, attr);
            }
            return attr.Cast<T>();
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, Attribute>> PropertyCustomAttributeCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, Attribute>>(() => new ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, Attribute>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, Attribute> PropertyCustomAttributeCachedMapping
        {
            get { return PropertyCustomAttributeCachedMappingLazy.Value; }
        }

        public static T GetCustomAttributeCached<T>(this PropertyInfo property, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            Attribute attr;
            var key = new Tuple<PropertyInfo, Type, bool>(property, attrType, inherit);
            if (!PropertyCustomAttributeCachedMapping.TryGetValue(key, out attr))
            {
                attr = property.GetCustomAttribute<T>(inherit);
                PropertyCustomAttributeCachedMapping.TryAdd(key, attr);
            }
            return attr as T;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, IEnumerable<Attribute>>> PropertyCustomAttributesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, IEnumerable<Attribute>>>(() => new ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, IEnumerable<Attribute>>(), LazyThreadSafetyMode.PublicationOnly);

        private static ConcurrentDictionary<Tuple<PropertyInfo, Type, bool>, IEnumerable<Attribute>> PropertyCustomAttributesCachedMapping
        {
            get { return PropertyCustomAttributesCachedMappingLazy.Value; }
        }

        public static IEnumerable<T> GetCustomAttributesCached<T>(this PropertyInfo property, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            IEnumerable<Attribute> attr;
            var key = new Tuple<PropertyInfo, Type, bool>(property, attrType, inherit);
            if (!PropertyCustomAttributesCachedMapping.TryGetValue(key, out attr))
            {
                attr = property.GetCustomAttributes<T>(inherit);
                PropertyCustomAttributesCachedMapping.TryAdd(key, attr);
            }
            return attr.Cast<T>();
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, Attribute>> MethodCustomAttributeCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, Attribute>>(() => new ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, Attribute>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, Attribute> MethodCustomAttributeCachedMapping
        {
            get { return MethodCustomAttributeCachedMappingLazy.Value; }
        }
        public static T GetCustomAttributeCached<T>(this MethodInfo method, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            Attribute attr;
            var key = new Tuple<MethodInfo, Type, bool>(method, attrType, inherit);
            if (!MethodCustomAttributeCachedMapping.TryGetValue(key, out attr))
            {
                attr = method.GetCustomAttribute<T>(inherit);
                MethodCustomAttributeCachedMapping.TryAdd(key, attr);
            }
            return attr as T;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, IEnumerable<Attribute>>> MethodCustomAttributesCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, IEnumerable<Attribute>>>(() => new ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, IEnumerable<Attribute>>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Tuple<MethodInfo, Type, bool>, IEnumerable<Attribute>> MethodCustomAttributesCachedMapping
        {
            get { return MethodCustomAttributesCachedMappingLazy.Value; }
        }

        public static IEnumerable<T> GetCustomAttributesCached<T>(this MethodInfo method, bool inherit = false)
            where T : Attribute
        {
            var attrType = typeof(T);
            IEnumerable<Attribute> attr;
            var key = new Tuple<MethodInfo, Type, bool>(method, attrType, inherit);
            if (!MethodCustomAttributesCachedMapping.TryGetValue(key, out attr))
            {
                attr = method.GetCustomAttributes<T>(inherit);
                MethodCustomAttributesCachedMapping.TryAdd(key, attr);
            }
            return attr.Cast<T>();
        }

        private static readonly Lazy<ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>>> AllPropertiesCachedMappingLazy = new Lazy<ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>>>(() => new ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>> AllPropertiesCachedMapping
        {
            get { return AllPropertiesCachedMappingLazy.Value; }
        }

        public static PropertyInfo GetPropertyCached(this Type type, string propertyName)
        {
            IReadOnlyDictionary<string, PropertyInfo> properties;
            if (!AllPropertiesCachedMapping.TryGetValue(type, out properties))
            {
                properties = (IReadOnlyDictionary<string, PropertyInfo>)type.GetPropertiesCached(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToDictionary(p => p.Name);
                AllPropertiesCachedMapping.TryAdd(type, properties);
            }
            PropertyInfo property;
            properties.TryGetValue(propertyName, out property);
            return property;
        }

        private static readonly Lazy<ConcurrentDictionary<Tuple<Type, Type>, bool>> HasInterfaceCachedMappingLazy = new Lazy<ConcurrentDictionary<Tuple<Type, Type>, bool>>(() => new ConcurrentDictionary<Tuple<Type, Type>, bool>(), LazyThreadSafetyMode.PublicationOnly);
        private static ConcurrentDictionary<Tuple<Type, Type>, bool> HasInterfaceCachedMapping
        {
            get { return HasInterfaceCachedMappingLazy.Value; }
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            bool hasInterface;
            Tuple<Type, Type> key = new Tuple<Type, Type>(type, interfaceType);
            if (!HasInterfaceCachedMapping.TryGetValue(key, out hasInterface))
            {
                hasInterface = type.GetInterfacesCached().Any(i => i == interfaceType);
                HasInterfaceCachedMapping.TryAdd(key, hasInterface);
            }
            return hasInterface;
        }
    }
}
