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
        public static void InjectPrefix<InjectionTarget, PrefixTarget>(string injectTargetMethod, string prefixTargetMethod, IMod mod)
        {
            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if (injectTarget == null)
                throw new MissingMethodException(typeof(InjectionTarget).Name, injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, prefixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to <paramref name="sourceMethod"/> to <paramref name="targetMethod"/> that will be called at the start of the method.
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethod">The method to inject a call to</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefix(MethodInfo targetMethod, MethodInfo sourceMethod, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetMethod, new HarmonyMethod(sourceMethod));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetMethod">The name of the method to inject into</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPostfix<InjectionTarget, PostfixTarget>(string injectTargetMethod, string postfixTargetMethod, IMod mod)
        {
            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if (injectTarget == null)
                throw new MissingMethodException(typeof(InjectionTarget).Name, injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget, null, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to sourceMethod to targetMethod that will be called at the end of the method.
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethod">The method to inject a call to</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPostfix(MethodInfo targetMethod, MethodInfo sourceMethod, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetMethod, null, new HarmonyMethod(sourceMethod));
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
        public static void InjectPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetMethod, string prefixTargetMethod, string postfixTargetMethod, IMod mod)
        {
            MethodInfo injectTarget = typeof(InjectionTarget).GetMethod(injectTargetMethod, FLAGS);
            if (injectTarget == null)
                throw new MissingMethodException(typeof(InjectionTarget).Name, injectTargetMethod);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, prefixTargetMethod);

            MethodInfo injectionPostfix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPostfix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget, new HarmonyMethod(injectionPrefix), new HarmonyMethod(injectionPostfix));
        }

        /// <summary>
        /// Adds a call to sourceMethodPrefix and sourceMethodPostfix to targetMethod
        /// </summary>
        /// <param name="targetMethod">The method to inject into</param>
        /// <param name="sourceMethodPrefix">The prefix method</param>
        /// <param name="sourceMethodPostfix">The postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectPrefixAndPostfix(MethodInfo targetMethod, MethodInfo sourceMethodPrefix, MethodInfo sourceMethodPostfix, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetMethod, new HarmonyMethod(sourceMethodPrefix), new HarmonyMethod(sourceMethodPostfix));
        }
        #endregion

        #region Get property
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, prefixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the start of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="prefixSource">The source of the injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefix(PropertyInfo targetProperty, MethodInfo prefixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetGetMethod(), new HarmonyMethod(prefixSource));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetGetMethod(), null, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="postfixSource">The source of the injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPostfix(PropertyInfo targetProperty, MethodInfo postfixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetGetMethod(), null, new HarmonyMethod(postfixSource));
        }

        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod and PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod
        /// </summary>
        /// <typeparam name="InjectionTarget">The declaring type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The declaring type of the prefix method</typeparam>
        /// <typeparam name="PostfixTarget">The declaring type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the property to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetProperty, string prefixTargetMethod, string postfixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetGetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, postfixTargetMethod);

            MethodInfo injectionPostfix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPostfix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetGetMethod(), new HarmonyMethod(injectionPrefix), new HarmonyMethod(injectionPostfix));
        }

        /// <summary>
        /// Adds a call to prefixSource and postfixSource to targetProperty
        /// </summary>
        /// <param name="targetProperty">The property to inject to</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectGetPropertyPrefixAndPostfix(PropertyInfo targetProperty, MethodInfo prefixSource, MethodInfo postfixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetGetMethod(), new HarmonyMethod(prefixSource), new HarmonyMethod(postfixSource));
        }
        #endregion

        #region Set property
        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the start of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefix<InjectionTarget, PrefixTarget>(string injectTargetProperty, string prefixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, prefixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the start of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefix(PropertyInfo targetProperty, MethodInfo prefixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetSetMethod(), new HarmonyMethod(prefixSource));
        }

        /// <summary>
        /// Adds a call to PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod that will be called at the end of the method.
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the method to inject into</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the method to inject into</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPostfix<InjectionTarget, PostfixTarget>(string injectTargetProperty, string postfixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetSetMethod(), null, new HarmonyMethod(injectionPrefix));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPostfix(PropertyInfo targetProperty, MethodInfo postfixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetSetMethod(), null, new HarmonyMethod(postfixSource));
        }

        /// <summary>
        /// Adds a call to PrefixTarget.prefixTargetMethod and PostfixTarget.postfixTargetMethod to InjectionTarget.injectTargetMethod
        /// </summary>
        /// <typeparam name="InjectionTarget">The type of the property to inject into</typeparam>
        /// <typeparam name="PrefixTarget">The type of the prefix method</typeparam>
        /// <typeparam name="PostfixTarget">The type of the postfix method</typeparam>
        /// <param name="injectTargetProperty">The name of the property to inject into</param>
        /// <param name="prefixTargetMethod">The name of the prefix method</param>
        /// <param name="postfixTargetMethod">The name of the postfix method</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefixAndPostfix<InjectionTarget, PrefixTarget, PostfixTarget>(string injectTargetProperty, string prefixTargetMethod, string postfixTargetMethod, IMod mod)
        {
            PropertyInfo injectTarget = typeof(InjectionTarget).GetProperty(injectTargetProperty, FLAGS);
            if (injectTarget == null || injectTarget.GetSetMethod() == null)
                throw new MissingMemberException(typeof(InjectionTarget).Name, injectTargetProperty);

            MethodInfo injectionPrefix = typeof(PrefixTarget).GetMethod(prefixTargetMethod, FLAGS);
            if (injectionPrefix == null)
                throw new MissingMethodException(typeof(PrefixTarget).Name, prefixTargetMethod);

            MethodInfo injectionPostfix = typeof(PostfixTarget).GetMethod(postfixTargetMethod, FLAGS);
            if (injectionPostfix == null)
                throw new MissingMethodException(typeof(PostfixTarget).Name, postfixTargetMethod);

            mod.HarmonyInstance.Patch(injectTarget.GetSetMethod(), new HarmonyMethod(injectionPrefix), new HarmonyMethod(injectionPostfix));
        }

        /// <summary>
        /// Adds a call to prefixSource to targetProperty that will be called at the end of the method.
        /// </summary>
        /// <param name="targetProperty">The property to inject into</param>
        /// <param name="prefixSource">The source of the prefix injection</param>
        /// <param name="postfixSource">The source of the postfix injection</param>
        /// <param name="mod">The owner of this injection</param>
        public static void InjectSetPropertyPrefixAndPostfix(PropertyInfo targetProperty, MethodInfo prefixSource, MethodInfo postfixSource, IMod mod)
        {
            mod.HarmonyInstance.Patch(targetProperty.GetSetMethod(), new HarmonyMethod(prefixSource), new HarmonyMethod(postfixSource));
        }
        #endregion
    }
}
    
