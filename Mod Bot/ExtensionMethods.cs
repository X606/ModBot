using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Dont call these methods directly from here
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Instead of having to filter the object array yourself you can use this method to get the object at a specific index in a much safer way
        /// </summary>
        /// <typeparam name="T">The type of the object at the index</typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index">The index of the object you want to get</param>
        /// <returns>The object at the specified index, casted to type <typeparamref name="T"/></returns>
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (index < 0 || index >= moddedObject.objects.Count)
                return null;

            if (!(moddedObject.objects[index] is T))
            {
                throw new InvalidCastException("Object at index " + index + " was not of type " + typeof(T).ToString());
            }

            return moddedObject.objects[index] as T;
        }

        /// <summary>
        /// Returns true of the mod is enbaled, false if its disabled
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool IsModEnabled(this Mod mod)
        {
            bool? isModDeactivated = ModsManager.Instance.IsModDeactivated(mod);

            if (!isModDeactivated.HasValue)
                throw new Exception("Mod \"" + mod.GetModName() + "\" with unique id \"" + mod.GetUniqueID() + "\" could not found in modsmanager's list of mods!");

            return !isModDeactivated.Value;
        }

        #region Mod Settings Extensions
        /// <summary>
        /// Sets the icon of the upgrade to a image from a url, this needs a internet connection (NOTE: this has a cache so if you want to change picture you might want to remove the cache in the mods directory)
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="url">the url of the image you want to set the object to</param>
        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.AddUpgradeIcon(upgradeDescription, url);
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name)
        {
            string result = OptionsSaver.LoadString(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static bool? GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name)
        {
            bool? result = OptionsSaver.LoadBool(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static float? GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name)
        {
            float? result = OptionsSaver.LoadFloat(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            int? result = OptionsSaver.LoadInt(mod, name);
            return result;
        }
        #endregion

        #region IEnumerable<T> Extensions
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

            if (Equals(field, null))
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

            if (Equals(property, null))
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

            if (Equals(methodInfo, null))
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

                if (Equals(methodInfo, null))
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

            if (Equals(methodInfo, null))
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

                if (Equals(methodInfo, null))
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
        #endregion

    }
}
