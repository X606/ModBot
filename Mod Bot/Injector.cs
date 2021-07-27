using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace ModLibrary
{
    /// <summary>
    /// Used to inject pre and post injections into a target method
    /// </summary>
    public static class Injector
    {
        /// <summary>
        /// The flags that should be used when getting the methods to inject
        /// </summary>
        internal const BindingFlags FLAGS = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        #region Normal methods
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefix<InjectionTarget, PrefixTarget>(string injectTargetMethod, string prefixTargetMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            instance.Patch(injectTarget, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to <paramref name="sourceMethod"/> to <paramref name="targetMethod"/> that will be called at the start of the method.
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethod">The method to inject a call to</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefix(MethodInfo targetMethod, MethodInfo sourceMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetMethod, new HarmonyMethod(sourceMethod));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPostfix<InjectionTarget, PostfixTarget>(string injectTargetMethod, string postfixTargetMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget, null, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to sourceMethod to targetMethod that will be called at the end of the method.
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethod">The method to inject a call to</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPostfix(MethodInfo targetMethod, MethodInfo sourceMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetMethod, null, new HarmonyMethod(sourceMethod));
        }

        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod and PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            MethodInfo injectionPostfix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if(injectionPostfix == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget, new HarmonyMethod(injectionPrefix), new HarmonyMethod(injectionPostfix));
        }

        /// <summary>
        /// Adds a call to sourceMethodPrefix and sourceMethodPostfix to targetMethod
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethodPrefix">The prefix method</param>
        /// <param name="sourceMethodPostfix">The postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefixAndPostfix(MethodInfo targetMethod, MethodInfo sourceMethodPrefix, MethodInfo sourceMethodPostfix, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetMethod, new HarmonyMethod(sourceMethodPrefix), new HarmonyMethod(sourceMethodPostfix));
        }
        #endregion

        #region Get property
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetProperty">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetProperty, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetProperty);

            instance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the start of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="prefixSource">The source of the injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefix(PropertyInfo targetProperty, PropertyInfo prefixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetGetMethod(), new HarmonyMethod(prefixSource.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetProperty">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetProperty, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PostfixTarget).GetProperty(postfixTargetProperty, FLAGS);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetProperty);

            instance.Patch(injectTarget.GetGetMethod(), null, new HarmonyMethod(injectionPrefix.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="postfixSource">The source of the injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPostfix(PropertyInfo targetProperty, PropertyInfo postfixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetGetMethod(), null, new HarmonyMethod(postfixSource.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod and PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetMethod, FLAGS);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetMethod, FLAGS);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            PropertyInfo injectionPostfix = typeof(PostfixTarget).GetProperty(postfixTargetMethod, FLAGS);
            if(injectionPostfix == null || injectionPostfix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix.GetGetMethod()), new HarmonyMethod(injectionPostfix.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource and postfixSource to targetProperty
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefixAndPostfix(PropertyInfo targetProperty, PropertyInfo prefixSource, PropertyInfo postfixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetGetMethod(), null, new HarmonyMethod(postfixSource.GetGetMethod()));
        }
        #endregion

        #region Set property
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetProperty">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetProperty, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetProperty, FLAGS);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetProperty);

            instance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the start of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefix(PropertyInfo targetProperty, PropertyInfo prefixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetSetMethod(), new HarmonyMethod(prefixSource.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetProperty">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetProperty, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PostfixTarget).GetProperty(postfixTargetProperty, FLAGS);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetProperty);

            instance.Patch(injectTarget.GetSetMethod(), null, new HarmonyMethod(injectionPrefix.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPostfix(PropertyInfo targetProperty, PropertyInfo postfixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetSetMethod(), null, new HarmonyMethod(postfixSource.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod and PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetMethod, FLAGS);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetMethod, FLAGS);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            PropertyInfo injectionPostfix = typeof(PostfixTarget).GetProperty(postfixTargetMethod, FLAGS);
            if(injectionPostfix == null || injectionPostfix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix.GetSetMethod()), new HarmonyMethod(injectionPostfix.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefixAndPostfix(PropertyInfo targetProperty, PropertyInfo prefixSource, PropertyInfo postfixSource, Mod mod)
        {
            Harmony instance = new Harmony(mod.HarmonyID);

            instance.Patch(targetProperty.GetSetMethod(), new HarmonyMethod(prefixSource.GetSetMethod()), new HarmonyMethod(postfixSource.GetSetMethod()));
        }
        #endregion
    }
}
    
