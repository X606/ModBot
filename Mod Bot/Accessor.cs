using System;
using System.Reflection;

namespace ModLibrary
{
    public class Accessor
    {
        /// <summary>
        /// The <see cref="Type"/> that defines the members that can be accessed
        /// </summary>
        public readonly Type InstanceType;

        /// <summary>
        /// The instance of the <see cref="Type"/> to get members from
        /// </summary>
        public readonly object Instance;

        /// <summary>
        /// The <see cref="BindingFlags"/> used to access members
        /// </summary>
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Defines a new instance of the <see cref="Accessor"/> class
        /// </summary>
        /// <param name="_type">The declaring <see cref="Type"/> that defines the members that should be accessed</param>
        /// <param name="_instance">The instance of the given <see cref="Type"/> to get members from</param>
        public Accessor(Type _type, object _instance)
        {
            InstanceType = _type;
            Instance = _instance;
        }

        /// <summary>
        /// Calls a private method in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <param name="methodName">The name of the method to call, case-sensitive</param>
        /// <param name="args">The arguments to pass to the method, <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the method</returns>
        public object CallPrivateMethod(string methodName, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = InstanceType.GetMethod(methodName, Flags);

            if (Equals(method, null))
            {
                throw new MissingMethodException(InstanceType.FullName, methodName);
            }

            return method.Invoke(Instance, args);
        }

        /// <summary>
        /// Calls a private method in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <typeparam name="ReturnType">The <see cref="Type"/> to cast the method's return value to</typeparam>
        /// <param name="methodName">The name of the method to call, case-sensitive</param>
        /// <param name="args">The arguments to pass to the method, <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the method casted to <typeparamref name="ReturnType"/></returns>
        public ReturnType CallPrivateMethod<ReturnType>(string methodName, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = InstanceType.GetMethod(methodName, Flags);

            if (Equals(method, null))
            {
                throw new MissingMethodException(InstanceType.FullName, methodName);
            }

            return (ReturnType)method.Invoke(Instance, args);
        }

        /// <summary>
        /// Sets a private field in the <see cref="Type"/> and instance specified in the constructor to a specified value
        /// </summary>
        /// <param name="fieldName">The name of the field to set, case-sensitive</param>
        /// <param name="value">The value to set the field to</param>
        public void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            field.SetValue(Instance, value);
        }

        /// <summary>
        /// Sets a private field in the <see cref="Type"/> and instance specified in the constructor to a specified value
        /// </summary>
        /// <typeparam name="FieldType">The <see cref="Type"/> of the field</typeparam>
        /// <param name="fieldName">The name of the field to set, case-sensitive</param>
        /// <param name="value">The value to set the field to</param>
        public void SetPrivateField<FieldType>(string fieldName, FieldType value)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            field.SetValue(Instance, value);
        }

        /// <summary>
        /// Gets a private field's value in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <param name="fieldName">The name of the field to get, case-sensitive</param>
        /// <returns>The value of the field</returns>
        public object GetPrivateField(string fieldName)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            return field.GetValue(Instance);
        }

        /// <summary>
        /// Gets a private field's value in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <typeparam name="FieldType">The <see cref="Type"/> of the field</typeparam>
        /// <param name="fieldName">The name of the field to get, case-sensitive</param>
        /// <returns>The value of the field casted to <typeparamref name="FieldType"/></returns>
        public FieldType GetPrivateField<FieldType>(string fieldName)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            return (FieldType)field.GetValue(Instance);
        }

        /// <summary>
        /// Sets a private property in the <see cref="Type"/> and instance specified in the constructor to a specified value
        /// </summary>
        /// <param name="propertyName">The name of the property, case-sensitive</param>
        /// <param name="value">The value to set the property to</param>
        public void SetPrivateProperty(string propertyName, object value)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            property.SetValue(Instance, value, null);
        }

        /// <summary>
        /// Sets a private property in the <see cref="Type"/> and instance specified in the constructor to a specified value
        /// </summary>
        /// <typeparam name="PropertyType">The <see cref="Type"/> of the property</typeparam>
        /// <param name="propertyName">The name of the property, case-sensitive</param>
        /// <param name="value">The value to set the property to</param>
        public void SetPrivateProperty<PropertyType>(string propertyName, PropertyType value)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            property.SetValue(Instance, value, null);
        }

        /// <summary>
        /// Gets a private property's value in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <param name="propertyName">The name of the property to get the value of, case-sensitive</param>
        /// <returns>The value of the proerty</returns>
        public object GetPrivateProperty(string propertyName)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            return property.GetValue(Instance, null);
        }

        /// <summary>
        /// Gets a private property's value in the <see cref="Type"/> and instance specified in the constructor
        /// </summary>
        /// <typeparam name="PropertyType">The <see cref="Type"/> of the property</typeparam>
        /// <param name="propertyName">The name of the property to get the value of, case-sensitive</param>
        /// <returns>The value of the proerty casted to <typeparamref name="PropertyType"/></returns>
        public PropertyType GetPrivateProperty<PropertyType>(string propertyName)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            return (PropertyType)property.GetValue(Instance, null);
        }

        /// <summary>
        /// Calls a method even if its private
        /// </summary>
        /// <param name="type">The type that the method is in. (Get this by typing "<see langword="typeof"/>(Class)" where "Class" is the <see langword="class"/> where the method you want to run is located).</param>
        /// <param name="methodName">The name of the method, case sensitive.</param>
        /// <param name="instance">The object that the method is attached to.</param>
        /// <param name="args">The arguments you want to pass in, if left empty no arguments will be called. Defined like this: <see langword="new object"/>[] { arg1, arg2, arg3 } </param>
        public static object CallPrivateMethod(Type type, string methodName, object instance, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = type.GetMethod(methodName, Flags);

            if (Equals(method, null))
            {
                throw new MissingMethodException(type.FullName, methodName);
            }

            return method.Invoke(instance, args);
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
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = typeof(InstanceType).GetMethod(methodName, Flags);

            if (Equals(method, null))
            {
                throw new MissingMethodException(typeof(InstanceType).FullName, methodName);
            }

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
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = typeof(InstanceType).GetMethod(methodName, Flags);

            if (Equals(method, null))
            {
                throw new MissingMethodException(typeof(InstanceType).FullName, methodName);
            }

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
            FieldInfo field = type.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(type.FullName, fieldName);
            }

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
            FieldInfo field = typeof(InstanceType).GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(typeof(InstanceType).FullName, fieldName);
            }

            field.SetValue(instance, value);
        }
        
        /// <summary>
        /// Gets field value even if its private.
        /// </summary>
        /// <param name="type">The type that the field is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="instance">The object that the field is attached to.</param>
        public static object GetPrivateField(Type type, string fieldName, object instance)
        {
            FieldInfo field = type.GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(type.FullName, fieldName);
            }

            return field.GetValue(instance);
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
            FieldInfo field = typeof(InstanceType).GetField(fieldName, Flags);

            if (Equals(field, null))
            {
                throw new MissingFieldException(typeof(InstanceType).FullName, fieldName);
            }

            return (FieldType)field.GetValue(instance);
        }
        
        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <param name="type">The type that the property is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the Property.</param>
        /// <param name="instance">The object that the property is attached to.</param>
        /// <param name="value">The value the property should be set to.</param>
        public static void SetPrivateProperty(Type type, string propertyName, object instance, object value)
        {
            PropertyInfo property = type.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(type.FullName, propertyName);
            }

            property.SetValue(instance, value, null);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the class the property is in.</typeparam>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertryName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value)
        {
            PropertyInfo property = typeof(InstanceType).GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(typeof(InstanceType).FullName, propertyName);
            }

            property.SetValue(instance, value, null);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <param name="type">The type that the property is in. (Get this by typing "typeof(Class)" where Class is the class where the mathod you want to run is located).</param>
        /// <param name="propertyName">The name of the Property.</param>
        /// <param name="instance">The object that the property is attached to.</param>
        public static object GetPrivateProperty(Type type, string propertyName, object instance)
        {
            PropertyInfo property = type.GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(type.FullName, propertyName);
            }

            return property.GetValue(instance, null);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <typeparam name="InstanceType">The type of the clas the property is in.</typeparam>
        /// <typeparam name="PropertyType">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="instance">The instance the property is in.</param>
        public static PropertyType GetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance)
        {
            PropertyInfo property = typeof(InstanceType).GetProperty(propertyName, Flags);

            if (Equals(property, null))
            {
                throw new MissingMemberException(typeof(InstanceType).FullName, propertyName);
            }

            return (PropertyType)property.GetValue(instance, null);
        }
    }
}
