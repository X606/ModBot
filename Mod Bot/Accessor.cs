using System;
using System.Reflection;

namespace ModLibrary
{
    /// <summary>
    /// Used to get / set / call private values and methods on objects
    /// </summary>
    public static class Accessor
    {
        /// <summary>
        /// The <see cref="BindingFlags"/> used to access members
        /// </summary>
        public const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        static Type[] getArgumentTypeArray(object[] args)
        {
            return args == null ? new Type[0] : Type.GetTypeArray(args);
        }

        static MethodInfo findMethodRecursive(Type instanceType, string methodName, object[] args)
        {
            MethodInfo methodInfo = instanceType.GetMethod(methodName, FLAGS, null, getArgumentTypeArray(args), null);

            if (Equals(methodInfo, null))
            {
                // Recursively go down the hierarchy to find the method
                if (instanceType.BaseType != null)
                {
                    methodInfo = findMethodRecursive(instanceType.BaseType, methodName, args);
                }
                else
                {
                    return null;
                }
            }

            return methodInfo;
        }
        static MethodInfo findMethod(Type instanceType, string methodName, object[] args)
        {
            MethodInfo methodInfo = findMethodRecursive(instanceType, methodName, args);
            if (Equals(methodInfo, null))
                throw new MissingMethodException(instanceType.FullName, methodName);

            return methodInfo;
        }

        /// <summary>
        /// Calls a method even if its private and returns its return value.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the method is in.</typeparam>
        /// <typeparam name="ReturnType">The return type of the method.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="instance">The instance of the class you want to call the method in.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public static ReturnType CallPrivateMethod<InstanceType, ReturnType>(string methodName, InstanceType instance, object[] args = null)
        {
            MethodInfo method = findMethod(typeof(InstanceType), methodName, args);
            return (ReturnType)method.Invoke(instance, args);
        }

        /// <summary>
        /// Calls a method even if its private and returns its return value.
        /// </summary>
        /// <typeparam name="ReturnType">The return type of the method.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="instance">The instance of the class you want to call the method in.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public static ReturnType CallPrivateMethod<ReturnType>(this object instance, string methodName, object[] args = null)
        {
            MethodInfo method = findMethod(instance.GetType(), methodName, args);
            return (ReturnType)method.Invoke(instance, args);
        }

        /// <summary>
        /// Calls a method even if its private but does not return its return value. (Use only for methods that have no return value or you dont need it)
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the method is in.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="instance">The instance of the class you want to call the method in.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public static void CallPrivateMethod<InstanceType>(string methodName, InstanceType instance, object[] args = null)
        {
            MethodInfo method = findMethod(typeof(InstanceType), methodName, args);
            method.Invoke(instance, args);
        }

        /// <summary>
        /// Calls a method even if its private but does not return its return value. (Use only for methods that have no return value or you dont need it)
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="instance">The instance of the class you want to call the method in.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public static void CallPrivateMethod(this object instance, string methodName, object[] args = null)
        {
            MethodInfo method = findMethod(instance.GetType(), methodName, args);
            method.Invoke(instance, args);
        }

        static FieldInfo findFieldRecursive(Type instanceType, string fieldName)
        {
            FieldInfo fieldInfo = instanceType.GetField(fieldName, FLAGS);

            if (Equals(fieldInfo, null))
            {
                // Recursively go down the hierarchy to find the field
                if (instanceType.BaseType != null)
                {
                    fieldInfo = findFieldRecursive(instanceType.BaseType, fieldName);
                }
                else
                {
                    return null;
                }
            }

            return fieldInfo;
        }
        static FieldInfo findField(Type instanceType, string fieldName)
        {
            FieldInfo fieldInfo = findFieldRecursive(instanceType, fieldName);
            if (Equals(fieldInfo, null))
                throw new MissingFieldException(instanceType.FullName, fieldName);

            return fieldInfo;
        }

        /// <summary>
        /// Sets field value even if its private.
        /// </summary>
        /// <param name="type">The type that the field is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="instance">The object that the field is attached to.</param>
        /// <param name="value">The value that the field should be set to.</param>
        public static void SetPrivateField(Type type, string fieldName, object instance, object value)
        {
            FieldInfo field = findField(type, fieldName);

            field.SetValue(instance, value);
        }

        /// <summary>
        /// Sets field value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the field is in.</typeparam>
        /// <typeparam name="FieldType">The type of the field.</typeparam>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="instance">The instance of the class the field is in.</param>
        /// <param name="value">The value to set the field to</param>
        public static void SetPrivateField<InstanceType, FieldType>(string fieldName, InstanceType instance, FieldType value)
        {
            FieldInfo field = findField(typeof(InstanceType), fieldName);

            field.SetValue(instance, value);
        }

        /// <summary>
        /// Sets field value even if its private.
        /// </summary>
        /// <typeparam name="FieldType">The type of the field.</typeparam>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="instance">The instance of the class the field is in.</param>
        /// <param name="value">The value to set the field to</param>
        public static void SetPrivateField<FieldType>(this object instance, string fieldName, FieldType value)
        {
            FieldInfo field = findField(instance.GetType(), fieldName);

            field.SetValue(instance, value);
        }

        /// <summary>
        /// Gets field value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the field is in.</typeparam>
        /// <typeparam name="FieldType">The type of the field.</typeparam>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="instance">The instance the field is in.</param>
        public static FieldType GetPrivateField<InstanceType, FieldType>(string fieldName, InstanceType instance)
        {
            FieldInfo field = findField(typeof(InstanceType), fieldName);

            return (FieldType)field.GetValue(instance);
        }

        /// <summary>
        /// Gets field value even if its private.
        /// </summary>
        /// <typeparam name="FieldType">The type of the field.</typeparam>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="instance">The instance the field is in.</param>
        public static FieldType GetPrivateField<FieldType>(this object instance, string fieldName)
        {
            FieldInfo field = findField(instance.GetType(), fieldName);

            return (FieldType)field.GetValue(instance);
        }

        static PropertyInfo findPropertyRecursive(Type instanceType, Type returnType, string fieldName, object[] args)
        {
            PropertyInfo propertyInfo = instanceType.GetProperty(fieldName, FLAGS, null, returnType, getArgumentTypeArray(args), null);

            if (Equals(propertyInfo, null))
            {
                // Recursively go down the hierarchy to find the property
                if (instanceType.BaseType != null)
                {
                    propertyInfo = findPropertyRecursive(instanceType.BaseType, returnType, fieldName, args);
                }
                else
                {
                    return null;
                }
            }

            return propertyInfo;
        }
        static PropertyInfo findProperty(Type instanceType, Type returnType, string propertyName, object[] args)
        {
            PropertyInfo propertyInfo = findPropertyRecursive(instanceType, returnType, propertyName, args);
            if (Equals(propertyInfo, null))
                throw new MissingMemberException(instanceType.FullName, propertyName);

            return propertyInfo;
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <param name="type">The type that the property is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="propertyName">The name of the Property.</param>
        /// <param name="instance">The object that the property is attached to.</param>
        /// <param name="value">The value the property should be set to.</param>
        public static void SetPrivateProperty(Type type, string propertyName, object instance, object value)
        {
            SetPrivateProperty(type, propertyName, instance, value, null);
        }

        /// <summary>
        /// Sets the value of a private property
        /// </summary>
        /// <param name="type">The <see cref="Type"/> declaring the property</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="instance">The instance to set the value on, can be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use on the property, pass <see langword="null"/> if property has no indexer</param>
        public static void SetPrivateProperty(Type type, string propertyName, object instance, object value, object[] indices)
        {
            PropertyInfo property = findProperty(type, value?.GetType(), propertyName, indices);

            property.SetValue(instance, value, indices);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the property is in.</typeparam>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value)
        {
            SetPrivateProperty(propertyName, instance, value, null);
        }

        /// <summary>
        /// Sets the value of a private property
        /// </summary>
        /// <typeparam name="InstanceType">The type declaring the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="instance">The instance to set the value on, can be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use on the property, pass <see langword="null"/> if property has no indexer</param>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value, object[] indices)
        {
            PropertyInfo property = findProperty(typeof(InstanceType), typeof(PropertyType), propertyName, indices);

            property.SetValue(instance, value, indices);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value)
        {
            SetPrivateProperty(instance, propertyName, value, null);
        }

        /// <summary>
        /// Sets the value of a private property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to set the value on></param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use on the property, pass <see langword="null"/> if property has no indexer</param>
        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value, object[] indices)
        {
            PropertyInfo property = findProperty(instance.GetType(), typeof(PropertyType), propertyName, indices);

            property.SetValue(instance, value, indices);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the clas the property is in.</typeparam>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        /// <param name="index">Used, if the property has a index parementer as the index parameter</param>
        public static PropertyType GetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, object[] index = null)
        {
            PropertyInfo property = findProperty(typeof(InstanceType), typeof(PropertyType), propertyName, index);

            return (PropertyType)property.GetValue(instance, index);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        /// <param name="index">Used, if the property has a index parementer as the index parameter</param>
        public static PropertyType GetPrivateProperty<PropertyType>(this object instance, string propertyName, object[] index = null)
        {
            PropertyInfo property = findProperty(instance.GetType(), typeof(PropertyType), propertyName, index);

            return (PropertyType)property.GetValue(instance, index);
        }
    }
}
