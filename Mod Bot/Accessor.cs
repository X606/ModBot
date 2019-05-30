using System;
using System.Reflection;

namespace ModLibrary
{
    public class Accessor
    {
        public readonly Type InstanceType;

        public readonly object Instance;

        public const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        public Accessor(Type _type, object _instance)
        {
            InstanceType = _type;
            Instance = _instance;
        }

        public object CallPrivateMethod(string methodName, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = InstanceType.GetMethod(methodName, Flags);

            if (object.Equals(method, null))
            {
                throw new MissingMethodException(InstanceType.FullName, methodName);
            }

            return method.Invoke(Instance, args);
        }

        public ReturnType CallPrivateMethod<ReturnType>(string methodName, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = InstanceType.GetMethod(methodName, Flags);

            if (object.Equals(method, null))
            {
                throw new MissingMethodException(InstanceType.FullName, methodName);
            }

            return (ReturnType)method.Invoke(Instance, args);
        }

        public void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (object.Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            field.SetValue(Instance, value);
        }

        public void SetPrivateField<FieldType>(string fieldName, FieldType value)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (object.Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            field.SetValue(Instance, value);
        }

        public object GetPrivateField(string fieldName)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (object.Equals(field, null))
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            return field.GetValue(Instance);
        }

        public FieldType GetPrivateField<FieldType>(string fieldName)
        {
            FieldInfo field = InstanceType.GetField(fieldName, Flags);

            if (field == null)
            {
                throw new MissingFieldException(InstanceType.FullName, fieldName);
            }

            return (FieldType)field.GetValue(Instance);
        }

        public void SetPrivateProperty(string propertyName, object value)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (object.Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            property.SetValue(Instance, value, null);
        }

        public void SetPrivateProperty<PropertyType>(string propertyName, PropertyType value)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (property == null)
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            property.SetValue(Instance, value, null);
        }

        public object GetPrivateProperty(string propertyName)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (object.Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            return property.GetValue(Instance, null);
        }

        public PropertyType GetPrivateProperty<PropertyType>(string propertyName)
        {
            PropertyInfo property = InstanceType.GetProperty(propertyName, Flags);

            if (object.Equals(property, null))
            {
                throw new MissingMemberException(InstanceType.FullName, propertyName);
            }

            return (PropertyType)property.GetValue(Instance, null);
        }

        /// <summary>
        /// Calls a Method even if its private
        /// </summary>
        /// <param name="type">The type that the method is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="methodName">The name of the method, case sensitive.</param>
        /// <param name="instance">The object that the method is attached to.</param>
        /// <param name="args">The arguments you want to pass in, if left empty no arguments will be called. Defined like this: object[] {arg1,arg2,arg3} </param>
        public static object CallPrivateMethod(Type type, string methodName, object instance, object[] args = null)
        {
            if (args == null)
            {
                args = new object[] { };
            }

            MethodInfo method = type.GetMethod(methodName, Flags);

            if (object.Equals(method, null))
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

            if (object.Equals(method, null))
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

            if (object.Equals(method, null))
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

            if (object.Equals(field, null))
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

            if (object.Equals(field, null))
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

            if (object.Equals(field, null))
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

            if (object.Equals(field, null))
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

            if (object.Equals(property, null))
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

            if (object.Equals(property, null))
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

            if (object.Equals(property, null))
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

            if (object.Equals(property, null))
            {
                throw new MissingMemberException(typeof(InstanceType).FullName, propertyName);
            }

            return (PropertyType)property.GetValue(instance, null);
        }
    }
}
