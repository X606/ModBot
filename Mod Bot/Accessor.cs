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
        public const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

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
            MethodInfo method = typeof(InstanceType).GetMethod(methodName, FLAGS, null, args == null ? new Type[0] : Type.GetTypeArray(args), null);

            if (Equals(method, null))
                throw new MissingMethodException(typeof(InstanceType).FullName, methodName);

            return (ReturnType)method.Invoke(instance, args);
        }

        public static ReturnType CallPrivateMethod<ReturnType>(this object instance, string methodName, object[] args = null)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, FLAGS, null, args == null ? new Type[0] : Type.GetTypeArray(args), null);

            if (Equals(method, null))
                throw new MissingMethodException(instance.GetType().FullName, methodName);

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
            MethodInfo method = typeof(InstanceType).GetMethod(methodName, FLAGS, null, args == null ? new Type[0] : Type.GetTypeArray(args), null);

            if (Equals(method, null))
                throw new MissingMethodException(typeof(InstanceType).FullName, methodName);

            method.Invoke(instance, args);
        }

        public static void CallPrivateMethod(this object instance, string methodName, object[] args = null)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, FLAGS, null, args == null ? new Type[0] : Type.GetTypeArray(args), null);

            if (Equals(method, null))
                throw new MissingMethodException(instance.GetType().FullName, methodName);

            method.Invoke(instance, args);
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
            FieldInfo field = type.GetField(fieldName, FLAGS);

            if (Equals(field, null))
                throw new MissingFieldException(type.FullName, fieldName);

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
            FieldInfo field = typeof(InstanceType).GetField(fieldName, FLAGS);

            if (Equals(field, null))
                throw new MissingFieldException(typeof(InstanceType).FullName, fieldName);

            field.SetValue(instance, value);
        }

        public static void SetPrivateField<FieldType>(this object instance, string fieldName, FieldType value)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, FLAGS);

            if (Equals(field, null))
                throw new MissingFieldException(instance.GetType().FullName, fieldName);

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
            FieldInfo field = typeof(InstanceType).GetField(fieldName, FLAGS);

            if (Equals(field, null))
                throw new MissingFieldException(typeof(InstanceType).FullName, fieldName);

            return (FieldType)field.GetValue(instance);
        }

        public static FieldType GetPrivateField<FieldType>(this object instance, string fieldName)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, FLAGS);

            if (Equals(field, null))
                throw new MissingFieldException(instance.GetType().FullName, fieldName);

            return (FieldType)field.GetValue(instance);
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
            PropertyInfo property = type.GetProperty(propertyName, FLAGS);

            if (Equals(property, null))
                throw new MissingMemberException(type.FullName, propertyName);

            property.SetValue(instance, value, null);
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
            PropertyInfo property = typeof(InstanceType).GetProperty(propertyName, FLAGS);

            if (Equals(property, null))
                throw new MissingMemberException(typeof(InstanceType).FullName, propertyName);

            property.SetValue(instance, value, null);
        }

        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName, FLAGS);

            if (Equals(property, null))
                throw new MissingMemberException(instance.GetType().FullName, propertyName);

            property.SetValue(instance, value, null);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the clas the property is in.</typeparam>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        public static PropertyType GetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, object[] index = null)
        {
            PropertyInfo property = typeof(InstanceType).GetProperty(propertyName, FLAGS);

            if (Equals(property, null))
                throw new MissingMemberException(typeof(InstanceType).FullName, propertyName);

            return (PropertyType)property.GetValue(instance, index);
        }

        public static PropertyType GetPrivateProperty<PropertyType>(this object instance, string propertyName, object[] index = null)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName, FLAGS);

            if (Equals(property, null))
                throw new MissingMemberException(instance.GetType().FullName, propertyName);

            return (PropertyType)property.GetValue(instance, null);
        }
    }
}
