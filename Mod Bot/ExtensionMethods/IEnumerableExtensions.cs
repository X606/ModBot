using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets all fields of the given name in the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="FieldType">The type to cast all the field values to</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to get the fields from</param>
        /// <param name="fieldName">The name of the field to get from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all the found fields with the given name casted to the given <typeparamref name="FieldType"/></returns>
        public static IEnumerable<FieldType> GetFields<CollectionType, FieldType>(this IEnumerable<CollectionType> collection, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            FieldInfo field = typeof(CollectionType).GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new MissingFieldException("Field \"" + typeof(CollectionType).Name + "." + fieldName + "\" could not be found!");
            }

            FieldType[] fields = new FieldType[collection.Count()];

            int currentIndex = 0;
            foreach (CollectionType item in collection)
            {
                fields[currentIndex] = (FieldType)field.GetValue(item);
                currentIndex++;
            }

            return fields;
        }

        /// <summary>
        /// Gets all properties of the given name in the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="PropertyType">The type to cast all the property values to</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to get the properties from</param>
        /// <param name="propertyName">The name of the property to get from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all the found properties with the given name casted to the given <typeparamref name="PropertyType"/></returns>
        public static IEnumerable<PropertyType> GetPropertyValues<CollectionType, PropertyType>(this IEnumerable<CollectionType> collection, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            PropertyInfo property = typeof(CollectionType).GetProperty(propertyName, bindingFlags);

            if (property == null)
            {
                throw new MissingMemberException("Property \"" + typeof(CollectionType).Name + "." + propertyName + "\" could not be found!");
            }

            PropertyType[] properties = new PropertyType[collection.Count()];

            int currentIndex = 0;
            foreach (CollectionType item in collection)
            {
                properties[currentIndex] = (PropertyType)property.GetValue(item);
                currentIndex++;
            }

            return properties;
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            CallMethods(collection, methodName, arguments: null, bindingFlags: bindingFlags);
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="arguments">The arguments to pass to the method</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, object[] arguments, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            Type[] argumentTypes;
            if (arguments != null)
            {
                argumentTypes = new Type[arguments.Length];
                argumentTypes = Type.GetTypeArray(arguments);
            }
            else
            {
                argumentTypes = new Type[0];
            }

            MethodInfo methodInfo = typeof(CollectionType).GetMethod(methodName, bindingFlags, null, argumentTypes, null);

            if (methodInfo == null)
            {
                string typeNames = string.Join(", ", argumentTypes.GetPropertyValues<Type, string>("Name"));

                throw new MissingMethodException("Method \"" + typeof(CollectionType).Name + "." + methodName + "(" + typeNames + ")\" could not be found!");
            }

            foreach (CollectionType item in collection)
            {
                methodInfo.Invoke(item, arguments);
            }
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="argumentFunction">The <see cref="Func{T, TResult}"/> used to get the argument values for each instance if the <see cref="IEnumerable{T}"/></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, Func<CollectionType, object[]> argumentFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            foreach (CollectionType item in collection)
            {
                object[] arguments = argumentFunction(item);
                Type[] argumentTypes = Type.GetTypeArray(arguments);

                MethodInfo methodInfo = typeof(CollectionType).GetMethod(methodName, bindingFlags, null, argumentTypes, null);
                
                if (methodInfo == null)
                {
                    string typeNames = string.Join(", ", argumentTypes.GetPropertyValues<Type, string>("Name"));

                    throw new MissingMethodException("Method \"" + typeof(CollectionType).Name + "." + methodName + "(" + typeNames + ")\" could not be found!");
                }

                methodInfo.Invoke(item, arguments);
            }
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <typeparam name="ReturnType">The return type of the method called</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <returns>The return values of all the methods called</returns>
        public static IEnumerable<KeyValuePair<CollectionType, ReturnType>> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return CallMethods<CollectionType, ReturnType>(collection, methodName, arguments: null, bindingFlags: bindingFlags);
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <typeparam name="ReturnType">The return type of the method called</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="arguments">The arguments to pass to all methods called</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <returns>The return values of all the methods called</returns>
        public static IEnumerable<KeyValuePair<CollectionType, ReturnType>> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, object[] arguments, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            Type[] argumentTypes;
            if (arguments != null)
            {
                argumentTypes = new Type[arguments.Length];
                argumentTypes = Type.GetTypeArray(arguments);
            }
            else
            {
                argumentTypes = new Type[0];
            }

            MethodInfo methodInfo = typeof(CollectionType).GetMethod(methodName, bindingFlags, null, argumentTypes, null);

            if (methodInfo == null)
            {
                string typeNames = string.Join(", ", argumentTypes.GetPropertyValues<Type, string>("Name"));

                throw new MissingMethodException("Method \"" + typeof(CollectionType).Name + "." + methodName + "(" + typeNames + ")\" could not be found!");
            }

            KeyValuePair<CollectionType, ReturnType>[] returnValues = new KeyValuePair<CollectionType, ReturnType>[collection.Count()];

            int currentIndex = 0;
            foreach (CollectionType item in collection)
            {
                object returnValue = methodInfo.Invoke(item, arguments);
                returnValues[currentIndex] = new KeyValuePair<CollectionType, ReturnType>(item, (ReturnType)returnValue);

                currentIndex++;
            }

            return returnValues;
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <typeparam name="ReturnType">The return type of the method called</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="argumentFunction">The <see cref="Func{T, TResult}"/> used to get the argument values for each instance if the <see cref="IEnumerable{T}"/></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <returns>The return values of all the methods called</returns>
        public static IEnumerable<KeyValuePair<CollectionType, ReturnType>> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, Func<CollectionType, object[]> argumentFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            KeyValuePair<CollectionType, ReturnType>[] returnValues = new KeyValuePair<CollectionType, ReturnType>[collection.Count()];

            int currentIndex = 0;
            foreach (CollectionType item in collection)
            {
                object[] arguments = argumentFunction(item);
                Type[] argumentTypes = Type.GetTypeArray(arguments);

                MethodInfo methodInfo = typeof(CollectionType).GetMethod(methodName, bindingFlags, null, argumentTypes, null);

                if (methodInfo == null)
                {
                    string typeNames = string.Join(", ", argumentTypes.GetPropertyValues<Type, string>("Name"));

                    throw new MissingMethodException("Method \"" + typeof(CollectionType).Name + "." + methodName + "(" + typeNames + ")\" could not be found!");
                }

                object returnValue = methodInfo.Invoke(item, arguments);
                returnValues[currentIndex] = new KeyValuePair<CollectionType, ReturnType>(item, (ReturnType)returnValue);

                currentIndex++;
            }

            return returnValues;
        }
    }
}
