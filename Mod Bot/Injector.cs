using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;

namespace ModLibrary
{
    /// <summary>
    /// Used to inject pre and post injections into a target method
    /// </summary>
    public static class Injector
    {
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
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            instance.Patch(injectTarget, new HarmonyMethod(injectionPrefix));
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
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget, null, new HarmonyMethod(injectionPrefix));
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
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            MethodInfo injectionPostfix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPostfix == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget, new HarmonyMethod(injectionPrefix), new HarmonyMethod(injectionPostfix));
        }
        #endregion

        #region Get poperty
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetProperty">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPopertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetProperty, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetProperty);

            instance.Patch(injectTarget.GetGetMethod(), null, new HarmonyMethod(injectionPrefix.GetGetMethod()));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetProperty">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPopertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetProperty, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PostfixTarget).GetProperty(postfixTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetProperty);

            instance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix.GetGetMethod()));
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
        public static void InjectGetPopertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            PropertyInfo injectionPostfix = typeof(PostfixTarget).GetProperty(postfixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPostfix == null || injectionPostfix.GetGetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix.GetGetMethod()), new HarmonyMethod(injectionPostfix.GetGetMethod()));
        }
        #endregion

        #region Set poperty
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetProperty">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPopertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetProperty, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetProperty);

            instance.Patch(injectTarget.GetSetMethod(), null, new HarmonyMethod(injectionPrefix.GetSetMethod()));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetProperty">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPopertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetProperty, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetProperty);

            PropertyInfo injectionPrefix = typeof(PostfixTarget).GetProperty(postfixTargetProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetProperty);

            instance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix.GetSetMethod()));
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
        public static void InjectSetPopertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, Mod mod)
        {
            HarmonyInstance instance = HarmonyInstance.Create(mod.HarmonyID);

            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(InjectionTarget).Name + "." + injectTargetMethod);

            PropertyInfo injectionPrefix = typeof(PrefixTarget).GetProperty(prefixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPrefix == null || injectionPrefix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PrefixTarget).Name + "." + prefixTargetMethod);

            PropertyInfo injectionPostfix = typeof(PostfixTarget).GetProperty(postfixTargetMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if(injectionPostfix == null || injectionPostfix.GetSetMethod() == null)
                throw new ArgumentException("Could not find " + typeof(PostfixTarget).Name + "." + postfixTargetMethod);

            instance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix.GetSetMethod()), new HarmonyMethod(injectionPostfix.GetGetMethod()));
        }
        #endregion
    }
}
    
