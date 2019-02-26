using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ModLibrary
{
    public class Accessor
    {
        public Accessor(Type _type, object _instance)
        {
            myType = _type;
            myInstance = _instance;
        }
        Type myType;
        object myInstance;
        public object CallPrivateMethod(string method, object[] args = null)
        {
            MethodInfo a = myType.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.Invoke(myInstance, args ?? (new object[] { }));
        }
        public MethodInfo[] GetAllMethods()
        {
            return myType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }
        [Obsolete("use SetPrivateField instead")]
        public void SetPriavteField(string name, object value)
        {
            SetPrivateField(name, value);
        }
        public void SetPrivateField(string name, object value)
        {
            FieldInfo a = myType.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            a.SetValue(myInstance, value);
        }
        public object GetPrivateField(string name)
        {
            FieldInfo a = myType.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.GetValue(myInstance);
        }
        public void SetPrivateProperty(string name, object value)
        {
            PropertyInfo a = myType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            a.SetValue(myInstance, value, null);
        }
        public object GetPrivateProperty(string name)
        {
            PropertyInfo a = myType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.GetValue(myInstance, null);
        }

        /// <summary>
        /// Calls a Method even if its private
        /// </summary>
        /// <param name="type">The type that the method is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="method">The name of the method, case sensitive.</param>
        /// <param name="instance">The object that the method is attached to.</param>
        /// <param name="args">The arguments you want to pass in, if left empty no arguments will be called. Defined like this: object[] {arg1,arg2,arg3} </param>
        public static object CallPrivateMethod(Type type, string method, object instance, object[] args = null)
        {
            MethodInfo a = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.Invoke(instance, args ?? (new object[] {}));
        }
        /// <summary>
        /// Just a debugging thing
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo[] GetAllMethods(Type type)
        {
            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Sets field value even if its private.
        /// </summary>
        /// <param name="type">The type that the field is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="instance">The object that the field is attached to.</param>
        /// <param name="value">The value that the field should be set to.</param>
        [Obsolete("use SetPrivateField instead")]
        public static void SetPriavteField(Type type, string name, object instance, object value)
        {
            SetPrivateField(type, name, instance, value);
        }
        /// <summary>
        /// Sets field value even if its private.
        /// </summary>
        /// <param name="type">The type that the field is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="instance">The object that the field is attached to.</param>
        /// <param name="value">The value that the field should be set to.</param>
        public static void SetPrivateField(Type type, string name, object instance, object value)
        {
            FieldInfo a = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            a.SetValue(instance, value);
        }


        /// <summary>
        /// Gets field value even if its private.
        /// </summary>
        /// <param name="type">The type that the field is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="instance">The object that the field is attached to.</param>
        public static object GetPrivateField(Type type, string name, object instance)
        {
            FieldInfo a = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.GetValue(instance);
        }


        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <param name="type">The type that the property is in. (Get this by typing "typeof(Class)" where Class is the class where the method you want to run is located).</param>
        /// <param name="name">The name of the Property.</param>
        /// <param name="instance">The object that the property is attached to.</param>
        /// <param name="value">The value the property should be set to.</param>
        public static void SetPrivateProperty(Type type, string name, object instance, object value)
        {
            PropertyInfo a = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            a.SetValue(instance, value, null);
        }

        /// <summary>
        /// Sets property value even if its private.
        /// </summary>
        /// <param name="type">The type that the property is in. (Get this by typing "typeof(Class)" where Class is the class where the mathod you want to run is located).</param>
        /// <param name="name">The name of the Property.</param>
        /// <param name="instance">The object that the property is attached to.</param>
        /// <param name="value">The value the property should be set to.</param>
        public static object GetPrivateProperty(Type type, string name, object instance)
        {
            PropertyInfo a = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return a.GetValue(instance,null);
        }
    }
}
