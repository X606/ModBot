using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Defines extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets all the <see cref="Component"/>s of the given type in each instance of the <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="ComponentType">The type of the <see cref="Component"/> to get, must enherit from <see cref="Component"/></typeparam>
        /// <param name="collection"></param>
        /// <returns>All components gotten from the <see cref="IEnumerable{T}"/></returns>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        public static IEnumerable<ComponentType> GetComponents<ComponentType>(this IEnumerable<Component> collection) where ComponentType : Component
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            ComponentType[] components = new ComponentType[collection.Count()];

            int currentIndex = 0;
            foreach (Component item in collection)
            {
                ComponentType component = item.GetComponent<ComponentType>();
                components[currentIndex] = component;

                currentIndex++;
            }

            return components;
        }

        /// <summary>
        /// Gets all fields of the given name in the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="FieldType">The type to cast all the field values to</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to get the fields from</param>
        /// <param name="fieldName">The name of the field to get from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all the found fields with the given name casted to the given <typeparamref name="FieldType"/></returns>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingFieldException">If the field could not be found</exception>
        public static IEnumerable<FieldType> GetFields<CollectionType, FieldType>(this IEnumerable<CollectionType> collection, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("Given field name was null or empty", nameof(fieldName));
            }

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
        /// Sets all fields of the given name in the given <see cref="IEnumerable{T}"/> to the given value
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to set all the fields in</param>
        /// <param name="fieldName">The name of the field to set from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="value">The value to set all the fields to</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingFieldException">If the field could not be found</exception>
        public static void SetFields<CollectionType, FieldType>(this IEnumerable<CollectionType> collection, string fieldName, FieldType value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("Given field name was null or empty", nameof(fieldName));
            }

            FieldInfo field = typeof(CollectionType).GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new MissingFieldException("Field \"" + typeof(CollectionType).Name + "." + fieldName + "\" could not be found!");
            }

            foreach (CollectionType item in collection)
            {
                field.SetValue(item, value);
            }
        }

        /// <summary>
        /// Sets all fields of the given name in the given <see cref="IEnumerable{T}"/> to the value gotten by calling the given <see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to set all the fields in</param>
        /// <param name="fieldName">The name of the field to set from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="valueFunction">The <see cref="Func{T, TResult}"/> used to get the value for each instance of the <see cref="IEnumerable{T}"/></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingFieldException">If the field could not be found</exception>
        public static void SetFields<CollectionType, FieldType>(this IEnumerable<CollectionType> collection, string fieldName, Func<CollectionType, FieldType> valueFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("Given field name was null or empty", nameof(fieldName));
            }
            if (valueFunction == null)
            {
                throw new ArgumentNullException(nameof(valueFunction));
            }

            FieldInfo field = typeof(CollectionType).GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new MissingFieldException("Field \"" + typeof(CollectionType).Name + "." + fieldName + "\" could not be found!");
            }

            foreach (CollectionType item in collection)
            {
                FieldType value = valueFunction(item);
                field.SetValue(item, value);
            }
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
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMemberException">If the property could not be found</exception>
        public static IEnumerable<PropertyType> GetPropertyValues<CollectionType, PropertyType>(this IEnumerable<CollectionType> collection, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Given property name was null or empty", nameof(propertyName));
            }

            PropertyInfo property = typeof(CollectionType).GetProperty(propertyName, bindingFlags, null, typeof(PropertyType), new Type[0], null);

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
        /// Sets all properties of the given name in the given <see cref="IEnumerable{T}"/> to the given value
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="PropertyType">The type of the property</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to set the properties in</param>
        /// <param name="propertyName">The name of the property to get from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="value">The value to set all the fields to</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMemberException">If the property could not be found</exception>
        public static void SetPropertyValues<CollectionType, PropertyType>(this IEnumerable<CollectionType> collection, string propertyName, PropertyType value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Given property name was null or empty", nameof(propertyName));
            }

            PropertyInfo property = typeof(CollectionType).GetProperty(propertyName, bindingFlags, null, typeof(PropertyType), new Type[0], null);

            if (property == null)
            {
                throw new MissingMemberException("Property \"" + typeof(CollectionType).Name + "." + propertyName + "\" could not be found!");
            }

            foreach (CollectionType item in collection)
            {
                property.SetValue(item, value);
            }
        }

        /// <summary>
        /// Sets all properties of the given name in the given <see cref="IEnumerable{T}"/> to the value gotten by calling the given <see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type if the original <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="PropertyType">The type of the property</typeparam>
        /// <param name="collection">>The <see cref="IEnumerable{T}"/> to set the properties in</param>
        /// <param name="propertyName">The name of the property to get from the <typeparamref name="CollectionType"/>, case-sensitive by default</param>
        /// <param name="valueFunction">The <see cref="Func{T, TResult}"/> used to get the value for each instance of the <see cref="IEnumerable{T}"/></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the fields</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMemberException">If the property could not be found</exception>
        public static void SetPropertyValues<CollectionType, PropertyType>(this IEnumerable<CollectionType> collection, string propertyName, Func<CollectionType, PropertyType> valueFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Given property name was null or empty", nameof(propertyName));
            }
            if (valueFunction == null)
            {
                throw new ArgumentNullException(nameof(valueFunction));
            }

            PropertyInfo property = typeof(CollectionType).GetProperty(propertyName, bindingFlags, null, typeof(PropertyType), new Type[0], null);

            if (property == null)
            {
                throw new MissingMemberException("Property \"" + typeof(CollectionType).Name + "." + propertyName + "\" could not be found!");
            }

            foreach (CollectionType item in collection)
            {
                PropertyType value = valueFunction(item);
                property.SetValue(item, value);
            }
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }

            CallMethods(collection, methodName, arguments: null, bindingFlags: bindingFlags);
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="arguments">The arguments to pass to the method, pass <see langword="null"/> for no arguments</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, object[] arguments, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }

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
        /// <param name="argumentFunction">The <see cref="Func{T, TResult}"/> used to get the argument values for each instance of the <see cref="IEnumerable{T}"/></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/>, or the <paramref name="argumentFunction"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static void CallMethods<CollectionType>(this IEnumerable<CollectionType> collection, string methodName, Func<CollectionType, object[]> argumentFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }
            if (argumentFunction == null)
            {
                throw new ArgumentNullException(nameof(argumentFunction));
            }

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
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static IEnumerable<ReturnType> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }

            return CallMethods<CollectionType, ReturnType>(collection, methodName, arguments: null, bindingFlags: bindingFlags);
        }

        /// <summary>
        /// Calls a method in all instances of the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the <see cref="IEnumerable{T}"/> to call the methods in</typeparam>
        /// <typeparam name="ReturnType">The return type of the method called</typeparam>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> to iterate through</param>
        /// <param name="methodName">The name of the method to call, case-sesitive by default</param>
        /// <param name="arguments">The arguments to pass to all methods called, pass <see langword="null"/> for no arguments</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used to find the methods</param>
        /// <returns>The return values of all the methods called</returns>
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static IEnumerable<ReturnType> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, object[] arguments, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }

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

            ReturnType[] returnValues = new ReturnType[collection.Count()];

            int currentIndex = 0;
            foreach (CollectionType item in collection)
            {
                object returnValue = methodInfo.Invoke(item, arguments);
                returnValues[currentIndex] = (ReturnType)returnValue;

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
        /// <exception cref="ArgumentNullException">If the given collection is <see langword="null"/>, or the <paramref name="argumentFunction"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given name string is empty, or <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">If the method could not be found</exception>
        public static IEnumerable<ReturnType> CallMethods<CollectionType, ReturnType>(this IEnumerable<CollectionType> collection, string methodName, Func<CollectionType, object[]> argumentFunction, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Given method name was null or empty", nameof(methodName));
            }
            if (argumentFunction == null)
            {
                throw new ArgumentNullException(nameof(argumentFunction));
            }

            ReturnType[] returnValues = new ReturnType[collection.Count()];

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
                returnValues[currentIndex] = (ReturnType)returnValue;

                currentIndex++;
            }

            return returnValues;
        }

        /// <summary>
        /// Gets a random item from the collection, or the default value for the type if the collection is empty
        /// </summary>
        /// <typeparam name="CollectionType">The type of the collection</typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the collection is <see langword="null"/></exception>
        public static CollectionType GetRandomOrDefault<CollectionType>(this IEnumerable<CollectionType> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            CollectionType[] collectionArray = collection.ToArray();

            if (collectionArray.Length == 0)
            {
                return default(CollectionType);
            }

            return collectionArray[UnityEngine.Random.Range(0, collectionArray.Length)];
        }

        /// <summary>
        /// Randomizes the order of elements in the given <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="CollectionType">The type of the collection to randomize</typeparam>
        /// <param name="enumerable"></param>
        /// <returns>The randomized collection</returns>
        public static IEnumerable<CollectionType> Randomize<CollectionType>(this IEnumerable<CollectionType> enumerable)
        {
            List<CollectionType> enumerableCopyList = enumerable.ToList();
            List<CollectionType> randomizedList = new List<CollectionType>();

            while (enumerableCopyList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, enumerableCopyList.Count);

                randomizedList.Add(enumerableCopyList[index]);
                enumerableCopyList.RemoveAt(index);
            }

            return randomizedList;
        }
    }
}
