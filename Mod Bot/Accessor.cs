using HarmonyLib;
using InternalModBot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModLibrary
{
    /// <summary>
    /// Helper class to access non-public members of types
    /// </summary>
    public static class Accessor
    {
        static readonly Dictionary<MemberInfoKey, MemberInfo> _accessorCache = new Dictionary<MemberInfoKey, MemberInfo>(MemberInfoKey.EqualityComparer);

        /// <summary>
        /// The default <see cref="BindingFlags"/> used to access members
        /// </summary>
        public const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        /// <summary>
        /// The <see cref="BindingFlags"/> used by the GetMemberInfo methods
        /// </summary>
        public const BindingFlags FLAGS_GETMEMBERINFO = FLAGS | BindingFlags.Public;

        internal static ParameterMatchType MatchType(Type type, MatchType argumentType)
        {
            // Null argument: any type goes, as long as it's nullable
            if (argumentType == null)
            {
                // If the parameter type is a value type, and it's not a Nullable<T>, it cannot be null and thus doesn't match
                if (type.IsClass || (type.IsValueType && Nullable.GetUnderlyingType(type) != null))
                {
                    return ParameterMatchType.PartialMatch;
                }
                else
                {
                    return ParameterMatchType.NoMatch;
                }
            }
            else if (!argumentType.IsAssignableTo(type))
            {
                return ParameterMatchType.NoMatch;
            }

            return ParameterMatchType.None;
        }

        internal static ParameterMatchType MatchParameterTypes(ParameterInfo[] parameters, MatchType[] argumentTypes, bool alwaysMatchIfNoArguments)
        {
            if (argumentTypes == null)
                argumentTypes = new MatchType[0];

            if (alwaysMatchIfNoArguments && argumentTypes.Length == 0)
                return ParameterMatchType.FullMatch;

            // Too many arguments: Can't match. Less arguments can match however, since not all arguments need to be explicitly stated if one or more of the parameters have default values
            if (argumentTypes.Length > parameters.Length)
                return ParameterMatchType.NoMatch;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i >= argumentTypes.Length)
                {
                    if (parameters[i].HasDefaultValue)
                    {
                        return ParameterMatchType.PartialMatch;
                    }
                    else
                    {
                        return ParameterMatchType.NoMatch;
                    }
                }

                ParameterMatchType parameterMatch = MatchType(parameters[i]?.ParameterType, argumentTypes[i]);
                if (parameterMatch != ParameterMatchType.None)
                    return parameterMatch;
            }

            return ParameterMatchType.FullMatch;
        }

        #region Method
        static MethodInfo findMethodRecursive(MethodInfoKey methodIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            MethodInfo foundMethodInfo = null;

            MethodInfo[] methodInfos = methodIdentifier.ReflectedType.GetMethods(flags);
            foreach (MethodInfo method in methodInfos)
            {
                if (method.Name == methodIdentifier.MemberName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    ParameterMatchType currentMatchType = MatchParameterTypes(parameters, methodIdentifier.ParameterTypes, alwaysMatchIfNoArguments);
                    if (currentMatchType >= ParameterMatchType.PartialMatch)
                    {
                        if (foundMethodInfo is null || foundMatchType < currentMatchType)
                        {
                            foundMethodInfo = method;
                            foundMatchType = currentMatchType;
                        }
                        else if (foundMatchType == currentMatchType)
                        {
                            throw new AmbiguousMatchException($"Ambiguous match between {method.GetFullDescription()} and {foundMethodInfo.GetFullDescription()}");
                        }
                    }
                }
            }

            if (foundMethodInfo is null && methodIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the type hierarchy to find the method
                foundMethodInfo = findMethodRecursive(methodIdentifier.StepDownInTypeHierarchy() as MethodInfoKey, flags, alwaysMatchIfNoArguments);
            }

            if (!(foundMethodInfo is null))
            {
                if (_accessorCache.TryGetValue(methodIdentifier, out MemberInfo cachedMemberInfo))
                {
                    if (cachedMemberInfo != foundMethodInfo)
                    {
                        throw new Exception($"MethodInfoKey {methodIdentifier} already present in dictionary, but results don't match! Cached: {cachedMemberInfo.GetFullDescription()}, Found: {foundMethodInfo.GetFullDescription()}");
                    }
                }
                else
                {
                    _accessorCache.Add(methodIdentifier, foundMethodInfo);
                }
            }

            return foundMethodInfo;
        }

        static MethodInfo findMethod(MethodInfoKey methodIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments, bool throwIfMissing)
        {
            if (_accessorCache.TryGetValue(methodIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is MethodInfo cachedMethodInfo)
                return cachedMethodInfo;

            MethodInfo methodInfo = findMethodRecursive(methodIdentifier, flags, alwaysMatchIfNoArguments);
            if (throwIfMissing && methodInfo is null)
                throw new MissingMethodException(methodIdentifier.ReflectedType.FullName, methodIdentifier.MemberName);

            return methodInfo;
        }

        /// <summary>
        /// Finds a <see cref="MethodInfo"/> which matches the arguments
        /// </summary>
        /// <param name="declaringType">The <see cref="Type"/> that declares the target method</param>
        /// <param name="methodName">The name of the target method (case-sensitive)</param>
        /// <param name="argumentTypes">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (Match any nullable type), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).</param>
        /// <returns>The found <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodInfo(this Type declaringType, string methodName, Type[] argumentTypes)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            return findMethod(new MethodInfoKey(declaringType, methodName, null, argumentTypes) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, false, false);
        }

        /// <summary>
        /// Finds a <see cref="MethodInfo"/> which matches the arguments
        /// </summary>
        /// <param name="declaringType">The <see cref="Type"/> that declares the target method</param>
        /// <param name="methodName">The name of the target method (case-sensitive)</param>
        /// <returns>The found <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodInfo(this Type declaringType, string methodName)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            return findMethod(new MethodInfoKey(declaringType, methodName, null, null) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, true, false);
        }

        /// <summary>
        /// Finds a <see cref="MethodInfo"/> which matches the arguments
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the target method</typeparam>
        /// <param name="methodName">The name of the target method (case-sensitive)</param>
        /// <param name="argumentTypes">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (Match any nullable type), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).</param>
        /// <returns>The found <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodInfo<InstanceType>(string methodName, Type[] argumentTypes)
        {
            return GetMethodInfo(typeof(InstanceType), methodName, argumentTypes);
        }

        /// <summary>
        /// Finds a <see cref="MethodInfo"/> which matches the arguments
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the target method</typeparam>
        /// <param name="methodName">The name of the target method (case-sensitive)</param>
        /// <returns>The found <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodInfo<InstanceType>(string methodName)
        {
            return GetMethodInfo(typeof(InstanceType), methodName);
        }

        /// <summary>
        /// Invokes a non-public instance method and returns its result
        /// </summary>
        /// <typeparam name="InstanceType">The type of the object used to invoke the method</typeparam>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<InstanceType, ReturnType>(string methodName, InstanceType instance, object[] args = null)
        {
            return CallPrivateMethod<InstanceType, ReturnType>(methodName, instance, args, null);
        }

        /// <summary>
        /// Invokes a non-public instance method and returns its result
        /// </summary>
        /// <typeparam name="InstanceType">The type of the object used to invoke the method</typeparam>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<InstanceType, ReturnType>(string methodName, InstanceType instance, object[] args, Type[] parameterTypeOverrides)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            return (ReturnType)findMethod(new MethodInfoKey(typeof(InstanceType), methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public instance method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(this object instance, string methodName, object[] args = null)
        {
            return CallPrivateMethod<ReturnType>(instance, methodName, args, null);
        }


        /// <summary>
        /// Invokes a non-public instance method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(this object instance, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            return (ReturnType)findMethod(new MethodInfoKey(instance.GetType(), methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="instance">The object used to invoke the method, type must be equivalent to the <paramref name="declaringType"/> parameter, to invoke a static method this parameter must be <see langword="null"/></param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(Type declaringType, object instance, string methodName, object[] args = null)
        {
            return CallPrivateMethod<ReturnType>(declaringType, instance, methodName, args, null);
        }


        /// <summary>
        /// Invokes a non-public method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="instance">The object used to invoke the method, type must be equivalent to the <paramref name="declaringType"/> parameter, to invoke a static method this parameter must be <see langword="null"/></param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(Type declaringType, object instance, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            return (ReturnType)findMethod(new MethodInfoKey(declaringType, methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public static method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(Type declaringType, string methodName, object[] args = null)
        {
            return CallPrivateMethod<ReturnType>(declaringType, methodName, args, null);
        }

        /// <summary>
        /// Invokes a non-public static method and returns its result
        /// </summary>
        /// <typeparam name="ReturnType">The target method's return value, only used to convert the return value from object to a target type</typeparam>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <returns>The return value of the target method</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static ReturnType CallPrivateMethod<ReturnType>(Type declaringType, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            return CallPrivateMethod<ReturnType>(declaringType, null, methodName, args, parameterTypeOverrides);
        }

        /// <summary>
        /// Invokes a non-public instance method and ignores its result
        /// </summary>
        /// <typeparam name="InstanceType">The type of the object used to invoke the method</typeparam>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod<InstanceType>(string methodName, InstanceType instance, object[] args = null)
        {
            CallPrivateMethod(methodName, instance, args, null);
        }

        /// <summary>
        /// Invokes a non-public instance method and ignores its result
        /// </summary>
        /// <typeparam name="InstanceType">The type of the object used to invoke the method</typeparam>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod<InstanceType>(string methodName, InstanceType instance, object[] args, Type[] parameterTypeOverrides)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            findMethod(new MethodInfoKey(typeof(InstanceType), methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public instance method and ignores its result
        /// </summary>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(this object instance, string methodName, object[] args = null)
        {
            CallPrivateMethod(instance, methodName, args, null);
        }

        /// <summary>
        /// Invokes a non-public instance method and ignores its result
        /// </summary>
        /// <param name="instance">The instance to invoke the method</param>
        /// <param name="methodName">The case-sensitive name of the target method</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(this object instance, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            findMethod(new MethodInfoKey(instance.GetType(), methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public method and ignores its result
        /// </summary>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="instance">The object used to invoke the method, type must be equivalent to the <paramref name="declaringType"/> parameter, to invoke a static method this parameter must be <see langword="null"/></param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(Type declaringType, object instance, string methodName, object[] args = null)
        {
            CallPrivateMethod(declaringType, instance, methodName, args, null);
        }

        /// <summary>
        /// Invokes a non-public method and ignores its result
        /// </summary>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="instance">The object used to invoke the method, type must be equivalent to the <paramref name="declaringType"/> parameter, to invoke a static method this parameter must be <see langword="null"/></param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(Type declaringType, object instance, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            findMethod(new MethodInfoKey(declaringType, methodName, args, parameterTypeOverrides), FLAGS, false, true).Invoke(instance, args);
        }

        /// <summary>
        /// Invokes a non-public static method and ignores its result
        /// </summary>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(Type declaringType, string methodName, object[] args = null)
        {
            CallPrivateMethod(declaringType, null, methodName, args);
        }

        /// <summary>
        /// Invokes a non-public static method and ignores its result
        /// </summary>
        /// <param name="declaringType">The declaring type of the target method</param>
        /// <param name="methodName">The name of the non-public method to invoke</param>
        /// <param name="args">The arguments to pass to the method, leave as <see langword="null"/> for no arguments</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="args"/> array</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingMethodException">The target method could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the method</exception>
        public static void CallPrivateMethod(Type declaringType, string methodName, object[] args, Type[] parameterTypeOverrides)
        {
            CallPrivateMethod(declaringType, null, methodName, args, parameterTypeOverrides);
        }
        #endregion

        #region Field
        static FieldInfo findFieldRecursive(FieldInfoKey fieldIdentifier, BindingFlags flags)
        {
            FieldInfo fieldInfo = fieldIdentifier.ReflectedType.GetField(fieldIdentifier.MemberName, flags);

            if (fieldInfo is null && fieldIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the type hierarchy to find the field
                fieldInfo = findFieldRecursive(fieldIdentifier.StepDownInTypeHierarchy() as FieldInfoKey, flags);
            }

            if (!(fieldInfo is null))
            {
                if (_accessorCache.TryGetValue(fieldIdentifier, out MemberInfo cachedMemberInfo))
                {
                    if (cachedMemberInfo != fieldInfo)
                    {
                        throw new Exception($"MemberInfoKey {fieldIdentifier} already present in dictionary, but results don't match! Cached: {cachedMemberInfo.GetFullDescription()}, Found: {fieldInfo.GetFullDescription()}");
                    }
                }
                else
                {
                    _accessorCache.Add(fieldIdentifier, fieldInfo);
                }
            }

            return fieldInfo;
        }
        static FieldInfo findField(FieldInfoKey fieldIdentifier, BindingFlags flags, bool throwIfMissing)
        {
            if (_accessorCache.TryGetValue(fieldIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is FieldInfo cachedFieldInfo)
                return cachedFieldInfo;

            FieldInfo fieldInfo = findFieldRecursive(fieldIdentifier, flags);
            if (throwIfMissing && fieldInfo is null)
                throw new MissingFieldException(fieldIdentifier.ReflectedType.FullName, fieldIdentifier.MemberName);

            return fieldInfo;
        }

        /// <summary>
        /// Finds a <see cref="FieldInfo"/> which matches the arguments
        /// </summary>
        /// <param name="declaringType">The <see cref="Type"/> that declares the target field</param>
        /// <param name="fieldName">The name of the target field (case-sensitive)</param>
        /// <returns>The found <see cref="FieldInfo"/></returns>
        public static FieldInfo GetFieldInfo(this Type declaringType, string fieldName)
        {
            return findField(new FieldInfoKey(declaringType, fieldName) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, false);
        }

        /// <summary>
        /// Finds a <see cref="FieldInfo"/> which matches the arguments
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the target field</typeparam>
        /// <param name="fieldName">The name of the target field (case-sensitive)</param>
        /// <returns>The found <see cref="FieldInfo"/></returns>
        public static FieldInfo GetFieldInfo<InstanceType>(string fieldName)
        {
            return findField(new FieldInfoKey(typeof(InstanceType), fieldName) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, false);
        }

        /// <summary>
        /// Sets the value of a non-public field
        /// </summary>
        /// <param name="declaringType">The type that declares the field</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="instance">The instance to assign the field on, if the target field is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The new value of the field</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static void SetPrivateField(Type declaringType, string fieldName, object instance, object value)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            findField(new FieldInfoKey(declaringType, fieldName), FLAGS, true).SetValue(instance, value);
        }

        /// <summary>
        /// Sets the value of a non-public static field
        /// </summary>
        /// <param name="declaringType">The type that declares the field</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="value">The new value of the field</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static void SetPrivateField(Type declaringType, string fieldName, object value)
        {
            SetPrivateField(declaringType, fieldName, null, value);
        }

        /// <summary>
        /// Sets the value of a non-public field
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the field</typeparam>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="instance">The instance to assign the field on</param>
        /// <param name="value">The new value of the field</param>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static void SetPrivateField<InstanceType, FieldType>(string fieldName, InstanceType instance, FieldType value)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            findField(new FieldInfoKey(typeof(InstanceType), fieldName), FLAGS, true).SetValue(instance, value);
        }

        /// <summary>
        /// Sets the value of a non-public instance field
        /// </summary>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="instance">The instance to assign the field on</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="value">The new value of the field</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static void SetPrivateField<FieldType>(this object instance, string fieldName, FieldType value)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            findField(new FieldInfoKey(instance.GetType(), fieldName), FLAGS, true).SetValue(instance, value);
        }

        /// <summary>
        /// Gets the value of a non-public field
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the field</typeparam>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="instance">The instance to read the field value from</param>
        /// <returns>The current value of the field</returns>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static FieldType GetPrivateField<InstanceType, FieldType>(string fieldName, InstanceType instance)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            return (FieldType)findField(new FieldInfoKey(typeof(InstanceType), fieldName), FLAGS, true).GetValue(instance);
        }

        /// <summary>
        /// Gets the value of a non-public instance field
        /// </summary>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="instance">The instance to read the field value from</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <returns>The current value of the field</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static FieldType GetPrivateField<FieldType>(this object instance, string fieldName)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            return (FieldType)findField(new FieldInfoKey(instance.GetType(), fieldName), FLAGS, true).GetValue(instance);
        }

        /// <summary>
        /// Gets the value of a non-public field
        /// </summary>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="declaringType">The type that declares the field</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <param name="instance">The instance to assign the field on, if the target field is static, this parameter should be <see langword="null"/></param>
        /// <returns>The current value of the field</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static FieldType GetPrivateField<FieldType>(Type declaringType, string fieldName, object instance)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            return (FieldType)findField(new FieldInfoKey(declaringType, fieldName), FLAGS, true).GetValue(instance);
        }

        /// <summary>
        /// Gets the value of a non-public static field
        /// </summary>
        /// <typeparam name="FieldType">The type of the field</typeparam>
        /// <param name="declaringType">The type that declares the field</param>
        /// <param name="fieldName">The case-sensitive name of the field</param>
        /// <returns>The current value of the field</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, empty, or consists only of whitespace characters</exception>
        /// <exception cref="MissingFieldException">The target field could not be found</exception>
        public static FieldType GetPrivateField<FieldType>(Type declaringType, string fieldName)
        {
            return GetPrivateField<FieldType>(declaringType, fieldName, null);
        }
        #endregion

        #region Property
        static PropertyInfo findPropertyRecursive(PropertyInfoKey propertyIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            PropertyInfo foundPropertyInfo = null;

            PropertyInfo[] propertyInfos = propertyIdentifier.ReflectedType.GetProperties(flags);
            foreach (PropertyInfo property in propertyInfos)
            {
                if (property.Name == propertyIdentifier.MemberName)
                {
                    if (propertyIdentifier.ReturnType == null || propertyIdentifier.ReturnType.IsAssignableFrom(property.PropertyType))
                    {
                        ParameterInfo[] parameters = property.GetIndexParameters();

                        ParameterMatchType currentMatchType = MatchParameterTypes(parameters, propertyIdentifier.IndexTypes, alwaysMatchIfNoArguments);
                        if (currentMatchType >= ParameterMatchType.PartialMatch)
                        {
                            if (foundPropertyInfo is null || foundMatchType < currentMatchType)
                            {
                                foundPropertyInfo = property;
                                foundMatchType = currentMatchType;
                            }
                            else if (foundMatchType == currentMatchType)
                            {
                                throw new AmbiguousMatchException($"Ambiguous match between {property.GetFullDescription()} and {foundPropertyInfo.GetFullDescription()}");
                            }
                        }
                    }
                }
            }

            if (foundPropertyInfo is null && propertyIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the hierarchy to find the property
                foundPropertyInfo = findPropertyRecursive(propertyIdentifier.StepDownInTypeHierarchy() as PropertyInfoKey, flags, alwaysMatchIfNoArguments);
            }

            if (!(foundPropertyInfo is null))
            {
                if (_accessorCache.TryGetValue(propertyIdentifier, out MemberInfo cachedMemberInfo))
                {
                    if (cachedMemberInfo != foundPropertyInfo)
                    {
                        throw new Exception($"PropertyInfoKey {propertyIdentifier} already present in dictionary, but results don't match! Cached: {cachedMemberInfo.GetFullDescription()}, Found: {foundPropertyInfo.GetFullDescription()}");
                    }
                }
                else
                {
                    _accessorCache.Add(propertyIdentifier, foundPropertyInfo);
                }
            }

            return foundPropertyInfo;
        }
        static PropertyInfo findProperty(PropertyInfoKey propertyIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments, bool throwIfMissing)
        {
            if (_accessorCache.TryGetValue(propertyIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is PropertyInfo cachedPropertyInfo)
                return cachedPropertyInfo;

            PropertyInfo propertyInfo = findPropertyRecursive(propertyIdentifier, flags, alwaysMatchIfNoArguments);
            if (throwIfMissing && propertyInfo is null)
                throw new MissingMemberException(propertyIdentifier.ReflectedType.FullName, propertyIdentifier.MemberName);

            return propertyInfo;
        }

        /// <summary>
        /// Finds a <see cref="PropertyInfo"/> which matches the arguments
        /// </summary>
        /// <param name="declaringType">The <see cref="Type"/> that declares the target property</param>
        /// <param name="propertyName">The name of the target property (case-sensitive)</param>
        /// <param name="argumentTypes">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (Match any nullable type), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).</param>
        /// <returns>The found <see cref="PropertyInfo"/></returns>
        public static PropertyInfo GetPropertyInfo(this Type declaringType, string propertyName, Type[] argumentTypes)
        {
            return findProperty(new PropertyInfoKey(declaringType, propertyName, null, null, argumentTypes) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, false, false);
        }

        /// <summary>
        /// Finds a <see cref="MethodInfo"/> which matches the arguments
        /// </summary>
        /// <param name="declaringType">The <see cref="Type"/> that declares the target property</param>
        /// <param name="propertyName">The name of the target property (case-sensitive)</param>
        /// <returns>The found <see cref="MethodInfo"/></returns>
        public static PropertyInfo GetPropertyInfo(this Type declaringType, string propertyName)
        {
            return findProperty(new PropertyInfoKey(declaringType, propertyName, null, null, null) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, true, false);
        }

        /// <summary>
        /// Finds a <see cref="PropertyInfo"/> which matches the arguments
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the target property</typeparam>
        /// <param name="propertyName">The name of the target property (case-sensitive)</param>
        /// <param name="argumentTypes">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (Match any nullable type), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).</param>
        /// <returns>The found <see cref="PropertyInfo"/></returns>
        public static PropertyInfo GetPropertyInfo<InstanceType>(string propertyName, Type[] argumentTypes)
        {
            return GetPropertyInfo(typeof(InstanceType), propertyName, argumentTypes);
        }

        /// <summary>
        /// Finds a <see cref="PropertyInfo"/> which matches the arguments
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the target property</typeparam>
        /// <param name="propertyName">The name of the target property (case-sensitive)</param>
        /// <returns>The found <see cref="PropertyInfo"/></returns>
        public static PropertyInfo GetPropertyInfo<InstanceType>(string propertyName)
        {
            return GetPropertyInfo(typeof(InstanceType), propertyName);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <param name="declaringType">The type that declares the property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty(Type declaringType, string propertyName, object instance, object value, object[] indices, Type[] parameterTypeOverrides)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            findProperty(new PropertyInfoKey(declaringType, propertyName, value?.GetType(), indices, parameterTypeOverrides), FLAGS, false, true).SetValue(instance, value);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <param name="declaringType">The type that declares the property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty(Type declaringType, string propertyName, object instance, object value)
        {
            SetPrivateProperty(declaringType, propertyName, instance, value, null, null);
        }

        /// <summary>
        /// Sets the value of a non-public static property
        /// </summary>
        /// <param name="declaringType">The type that declares the property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty(Type declaringType, string propertyName, object value)
        {
            SetPrivateProperty(declaringType, propertyName, null, value);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <param name="declaringType">The type that declares the property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty(Type declaringType, string propertyName, object instance, object value, object[] indices)
        {
            SetPrivateProperty(declaringType, propertyName, instance, value, indices, null);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value)
        {
            SetPrivateProperty(typeof(InstanceType), propertyName, instance, value, null, null);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value, object[] indices)
        {
            SetPrivateProperty(typeof(InstanceType), propertyName, instance, value, indices, null);
        }

        /// <summary>
        /// Sets the value of a non-public property
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, PropertyType value, object[] indices, Type[] parameterTypeOverrides)
        {
            SetPrivateProperty(typeof(InstanceType), propertyName, instance, value, indices, parameterTypeOverrides);
        }

        /// <summary>
        /// Sets the value of a non-public instance property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to set the property on</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> in <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            SetPrivateProperty(instance.GetType(), propertyName, instance, value, null, null);
        }

        /// <summary>
        /// Sets the value of a non-public instance property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to set the property on</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> in <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value, object[] indices)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            SetPrivateProperty(instance.GetType(), propertyName, instance, value, indices, null);
        }

        /// <summary>
        /// Sets the value of a non-public instance property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to set the property on</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="indices">The indices to use when invoking the property set method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> in <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static void SetPrivateProperty<PropertyType>(this object instance, string propertyName, PropertyType value, object[] indices, Type[] parameterTypeOverrides)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            SetPrivateProperty(instance.GetType(), propertyName, instance, value, indices, parameterTypeOverrides);
        }

        /// <summary>
        /// Gets the value of a non-public property
        /// </summary>
        /// <param name="declaringType">The type the declares the target property</param>
        /// <param name="instance">The instance to get the property from, if the target property is <see langword="static"/>, this parameter shoud be <see langword="null"/></param>
        /// <param name="returnType">The return type of the target property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static object GetPrivateProperty(Type declaringType, string propertyName, object instance, Type returnType, object[] indices, Type[] parameterTypeOverrides)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));

            if (instance != null && !declaringType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"'{nameof(instance)}' ({instance.GetType().FullDescription()}) must be implicitly assignable to '{nameof(declaringType)}' ({declaringType.FullDescription()})");

            return findProperty(new PropertyInfoKey(declaringType, propertyName, returnType, indices, parameterTypeOverrides), FLAGS, false, true).GetValue(instance, indices);
        }

        /// <summary>
        /// Gets the value of a non-public property
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to get the property from, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, object[] indices = null)
        {
            return (PropertyType)GetPrivateProperty(typeof(InstanceType), propertyName, instance, typeof(PropertyType), indices, null);
        }

        /// <summary>
        /// Gets the value of a non-public property
        /// </summary>
        /// <typeparam name="InstanceType">The type that declares the property</typeparam>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="instance">The instance to get the property from, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<InstanceType, PropertyType>(string propertyName, InstanceType instance, object[] indices, Type[] parameterTypeOverrides)
        {
            return (PropertyType)GetPrivateProperty(typeof(InstanceType), propertyName, instance, typeof(PropertyType), indices, parameterTypeOverrides);
        }

        /// <summary>
        /// Gets the value of a non-public instance property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to get the property from</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(this object instance, string propertyName, object[] indices = null)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            return (PropertyType)GetPrivateProperty(instance.GetType(), propertyName, instance, typeof(PropertyType), indices, null);
        }

        /// <summary>
        /// Gets the value of a non-public instance property
        /// </summary>
        /// <typeparam name="PropertyType">The return type of the property</typeparam>
        /// <param name="instance">The instance to get the property from</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(this object instance, string propertyName, object[] indices, Type[] parameterTypeOverrides)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            return (PropertyType)GetPrivateProperty(instance.GetType(), propertyName, instance, typeof(PropertyType), indices, parameterTypeOverrides);
        }

        /// <summary>
        /// Gets the value of a non-public property
        /// </summary>
        /// <typeparam name="PropertyType">The type returned by the property</typeparam>
        /// <param name="declaringType">The type that declares the target property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(Type declaringType, object instance, string propertyName, object[] indices = null)
        {
            return (PropertyType)GetPrivateProperty(declaringType, propertyName, instance, typeof(PropertyType), indices, null);
        }

        /// <summary>
        /// Gets the value of a non-public property
        /// </summary>
        /// <typeparam name="PropertyType">The type returned by the property</typeparam>
        /// <param name="declaringType">The type that declares the target property</param>
        /// <param name="instance">The instance to set the property on, if the target property is static, this parameter should be <see langword="null"/></param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type implicitly assignable to <paramref name="declaringType"/></exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(Type declaringType, object instance, string propertyName, object[] indices, Type[] parameterTypeOverrides)
        {
            return (PropertyType)GetPrivateProperty(declaringType, propertyName, instance, typeof(PropertyType), indices, parameterTypeOverrides);
        }

        /// <summary>
        /// Gets the value of a non-public static property
        /// </summary>
        /// <typeparam name="PropertyType">The type returned by the property</typeparam>
        /// <param name="declaringType">The type that declares the target property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(Type declaringType, string propertyName, object[] indices = null)
        {
            return (PropertyType)GetPrivateProperty(declaringType, propertyName, null, typeof(PropertyType), indices, null);
        }

        /// <summary>
        /// Gets the value of a non-public static property
        /// </summary>
        /// <typeparam name="PropertyType">The type returned by the property</typeparam>
        /// <param name="declaringType">The type that declares the target property</param>
        /// <param name="propertyName">The case-sensitive name of the property</param>
        /// <param name="indices">The indices to use when invoking the property get method, if the target property has no indexer parameters, this parameter should be <see langword="null"/></param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="indices"/> array</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, empty, or consist only of whitespace characters</exception>
        /// <exception cref="MissingMemberException">The target property could not be found</exception>
        /// <exception cref="AmbiguousMatchException">An ambiguous match was found while searching for the property</exception>
        public static PropertyType GetPrivateProperty<PropertyType>(Type declaringType, string propertyName, object[] indices, Type[] parameterTypeOverrides)
        {
            return (PropertyType)GetPrivateProperty(declaringType, propertyName, null, typeof(PropertyType), indices, parameterTypeOverrides);
        }
        #endregion

        #region Constructor
        static ConstructorInfo findConstructorRecursive(ConstructorInfoKey constructorIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            ConstructorInfo foundConstructorInfo = null;

            ConstructorInfo[] constructorInfos = constructorIdentifier.ReflectedType.GetConstructors(flags);
            foreach (ConstructorInfo constructor in constructorInfos)
            {
                // IsStatic = is type initializer
                if (constructorIdentifier.IsTypeInitializer == constructor.IsStatic)
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    ParameterMatchType currentMatchType = MatchParameterTypes(parameters, constructorIdentifier.ParameterTypes, alwaysMatchIfNoArguments);
                    if (currentMatchType >= ParameterMatchType.PartialMatch)
                    {
                        if (foundConstructorInfo is null || foundMatchType < currentMatchType)
                        {
                            foundConstructorInfo = constructor;
                            foundMatchType = currentMatchType;
                        }
                        else if (foundMatchType == currentMatchType)
                        {
                            throw new AmbiguousMatchException($"Ambiguous match between {constructor.GetFullDescription()} and {foundConstructorInfo.GetFullDescription()}");
                        }
                    }
                }
            }

            if (foundConstructorInfo is null && constructorIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the type hierarchy to find the constructor
                foundConstructorInfo = findConstructorRecursive(constructorIdentifier.StepDownInTypeHierarchy() as ConstructorInfoKey, flags, alwaysMatchIfNoArguments);
            }

            if (!(foundConstructorInfo is null))
            {
                if (_accessorCache.TryGetValue(constructorIdentifier, out MemberInfo cachedMemberInfo))
                {
                    if (cachedMemberInfo != foundConstructorInfo)
                    {
                        throw new Exception($"ConstructorInfoKey {constructorIdentifier} already present in dictionary, but results don't match! Cached: {cachedMemberInfo.GetFullDescription()}, Found: {foundConstructorInfo.GetFullDescription()}");
                    }
                }
                else
                {
                    _accessorCache.Add(constructorIdentifier, foundConstructorInfo);
                }
            }

            return foundConstructorInfo;
        }

        static ConstructorInfo findConstructor(ConstructorInfoKey constructorIdentifier, BindingFlags flags, bool alwaysMatchIfNoArguments, bool throwIfMissing)
        {
            if (_accessorCache.TryGetValue(constructorIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is ConstructorInfo cachedConstructorInfo)
                return cachedConstructorInfo;

            ConstructorInfo constructorInfo = findConstructorRecursive(constructorIdentifier, flags, alwaysMatchIfNoArguments);
            if (throwIfMissing && constructorInfo is null)
                throw new MissingMethodException(constructorIdentifier.ReflectedType.FullName, constructorIdentifier.MemberName);
            
            return constructorInfo;
        }

        /// <summary>
        /// Gets the static constructor declared in the given type.
        /// </summary>
        /// <param name="declaringType">The type that declares the target static constructor</param>
        /// <returns>The <see cref="ConstructorInfo"/> representing the <paramref name="declaringType"/>'s static constructor</returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        public static ConstructorInfo GetStaticConstructorInfo(this Type declaringType)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            return findConstructor(new ConstructorInfoKey(declaringType, true, null, null) { IsGetMemberInfoKey = true }, FLAGS, true, false); // static constructors cannot be public, so there is no reason to use FLAGS_GETMEMBERINFO
        }

        /// <summary>
        /// Gets the static constructor declared in the given type.
        /// </summary>
        /// <typeparam name="T">The type that declares the target static constructor</typeparam>
        /// <returns>The <see cref="ConstructorInfo"/> representing the <typeparamref name="T"/>'s static constructor</returns>
        public static ConstructorInfo GetStaticConstructorInfo<T>()
        {
            return GetStaticConstructorInfo(typeof(T));
        }

        /// <summary>
        /// Returns the single <see cref="ConstructorInfo"/> defined in <paramref name="declaringType"/>
        /// </summary>
        /// <param name="declaringType">The type that defines the target constructor</param>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="AmbiguousMatchException"><paramref name="declaringType"/> defines more than one constructor</exception>
        public static ConstructorInfo GetConstructorInfo(this Type declaringType)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            return findConstructor(new ConstructorInfoKey(declaringType, false, null, null) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, true, false);
        }

        /// <summary>
        /// Returns the single <see cref="ConstructorInfo"/> defined in <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type that defines the target constructor</typeparam>
        /// <exception cref="AmbiguousMatchException"><typeparamref name="T"/> defines more than one constructor</exception>
        public static ConstructorInfo GetConstructorInfo<T>()
        {
            return GetConstructorInfo(typeof(T));
        }

        /// <summary>
        /// Finds a <see cref="ConstructorInfo"/> defined in <paramref name="declaringType"/> with matching argument types <paramref name="argumentTypes"/>
        /// </summary>
        /// <param name="declaringType">The type that defines the target constructor</param>
        /// <param name="argumentTypes">The types of all the parameters in the target constructor</param>
        /// <returns>The found <see cref="ConstructorInfo"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static ConstructorInfo GetConstructorInfo(this Type declaringType, Type[] argumentTypes)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            return findConstructor(new ConstructorInfoKey(declaringType, false, null, argumentTypes) { IsGetMemberInfoKey = true }, FLAGS_GETMEMBERINFO, false, false);
        }

        /// <summary>
        /// Finds a <see cref="ConstructorInfo"/> defined in <typeparamref name="T"/> with matching argument types <paramref name="argumentTypes"/>
        /// </summary>
        /// <typeparam name="T">The type that defines the target constructor</typeparam>
        /// <param name="argumentTypes">The types of all the parameters in the target constructor</param>
        /// <returns>The found <see cref="ConstructorInfo"/></returns>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static ConstructorInfo GetConstructorInfo<T>(Type[] argumentTypes)
        {
            return GetConstructorInfo(typeof(T), argumentTypes);
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <paramref name="declaringType"/> that takes either no parameters or all parameters are optional
        /// </summary>
        /// <param name="declaringType">The type that defines the target constructor</param>
        /// <returns>The created instance of <paramref name="declaringType"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="AmbiguousMatchException"><paramref name="declaringType"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><paramref name="declaringType"/> does not define a constructor that matches the parameters</exception>
        public static object InvokePrivateConstructor(this Type declaringType)
        {
            return InvokePrivateConstructor(declaringType, null);
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <typeparamref name="T"/> that takes either no parameters or all parameters are optional
        /// </summary>
        /// <typeparam name="T">The type that defines the target constructor</typeparam>
        /// <returns>The created instance of <typeparamref name="T"/></returns>
        /// <exception cref="AmbiguousMatchException"><typeparamref name="T"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><typeparamref name="T"/> does not define a constructor that matches the parameters</exception>
        public static T InvokePrivateConstructor<T>()
        {
            return (T)InvokePrivateConstructor(typeof(T));
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <paramref name="declaringType"/> that takes parameters matching the types and order of <paramref name="arguments"/>
        /// </summary>
        /// <param name="declaringType">The type that defines the target constructor</param>
        /// <param name="arguments">The arguments to pass to the constructor</param>
        /// <returns>The created instance of <paramref name="declaringType"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="AmbiguousMatchException"><paramref name="declaringType"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><paramref name="declaringType"/> does not define a constructor that matches the parameters</exception>
        public static object InvokePrivateConstructor(this Type declaringType, params object[] arguments)
        {
            return InvokePrivateConstructor(declaringType, arguments, null);
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <typeparamref name="T"/> that takes parameters matching the types and order of <paramref name="arguments"/>
        /// </summary>
        /// <typeparam name="T">The type that defines the target constructor</typeparam>
        /// <param name="arguments">The arguments to pass to the constructor</param>
        /// <returns>The created instance of <typeparamref name="T"/></returns>
        /// <exception cref="AmbiguousMatchException"><typeparamref name="T"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><typeparamref name="T"/> does not define a constructor that matches the parameters</exception>
        public static T InvokePrivateConstructor<T>(params object[] arguments)
        {
            return (T)InvokePrivateConstructor(typeof(T), arguments);
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <paramref name="declaringType"/> that takes parameters matching the types and order of <paramref name="arguments"/> with optional type overrides defined with <paramref name="parameterTypeOverrides"/>
        /// </summary>
        /// <param name="declaringType">The type that defines the target constructor</param>
        /// <param name="arguments">The arguments to pass to the constructor</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="arguments"/> array</param>
        /// <returns>The created instance of <paramref name="declaringType"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/></exception>
        /// <exception cref="AmbiguousMatchException"><paramref name="declaringType"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><paramref name="declaringType"/> does not define a constructor that matches the parameters</exception>
        public static object InvokePrivateConstructor(this Type declaringType, object[] arguments, Type[] parameterTypeOverrides)
        {
            if (declaringType is null)
                throw new ArgumentNullException(nameof(declaringType));

            return findConstructor(new ConstructorInfoKey(declaringType, false, arguments, parameterTypeOverrides), FLAGS, false, true).Invoke(arguments);
        }

        /// <summary>
        /// Invokes the non-public constructor defined in <typeparamref name="T"/> that takes parameters matching the types and order of <paramref name="arguments"/> with optional type overrides defined with <paramref name="parameterTypeOverrides"/>
        /// </summary>
        /// <typeparam name="T">The type that defines the target constructor</typeparam>
        /// <param name="arguments">The arguments to pass to the constructor</param>
        /// <param name="parameterTypeOverrides">If not <see langword="null"/>, explicitly defines the type of specific arguments, individual items can be <see langword="null"/> (determine from context), or any <see cref="Type"/> (the argument at the same position as in this array will require a parameter that exactly matches that type).
        /// <br/>
        /// If <see langword="null"/>, all parameter types are decided by the items in the <paramref name="arguments"/> array</param>
        /// <returns>The created instance of <typeparamref name="T"/></returns>
        /// <exception cref="AmbiguousMatchException"><typeparamref name="T"/> defines more than one constructor that matches the parameters</exception>
        /// <exception cref="MissingMethodException"><typeparamref name="T"/> does not define a constructor that matches the parameters</exception>
        public static T InvokePrivateConstructor<T>(object[] arguments, Type[] parameterTypeOverrides)
        {
            return (T)InvokePrivateConstructor(typeof(T), arguments, parameterTypeOverrides);
        }
        #endregion

        #region Type
        static Type findNestedType(NestedTypeInfoKey typeIdentifier, BindingFlags flags, bool throwIfMissing)
        {
            if (_accessorCache.TryGetValue(typeIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is Type cachedType)
                return cachedType;

            Type nestedType = typeIdentifier.ReflectedType.GetNestedType(typeIdentifier.MemberName, flags);
            if (throwIfMissing && nestedType is null)
                throw new MissingMemberException(typeIdentifier.ReflectedType.FullName, typeIdentifier.MemberName);
            
            return nestedType;
        }

        /// <summary>
        /// Finds a nested type defined in <paramref name="declaringType"/> with name <paramref name="nestedTypeName"/>
        /// </summary>
        /// <param name="declaringType">The type that defines the target nested type</param>
        /// <param name="nestedTypeName">The name of the nested type</param>
        /// <returns>The nested type</returns>
        public static Type GetDeclaredNestedType(this Type declaringType, string nestedTypeName)
        {
            return findNestedType(new NestedTypeInfoKey(declaringType, nestedTypeName), FLAGS_GETMEMBERINFO, false);
        }
        #endregion

        /*
        #region Event
        static EventInfo findEventRecursive(EventInfoKey eventIdentifier, BindingFlags flags)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            EventInfo foundEventInfo = null;

            EventInfo[] eventInfos = eventIdentifier.ReflectedType.GetEvents(flags);
            foreach (EventInfo evnt in eventInfos)
            {
                if (evnt.Name == eventIdentifier.MemberName)
                {
                    ParameterMatchType currentMatchType = MatchType(evnt.EventHandlerType, eventIdentifier.DelegateType);
                    if (currentMatchType >= ParameterMatchType.PartialMatch)
                    {
                        if (foundEventInfo is null || foundMatchType < currentMatchType)
                        {
                            foundEventInfo = evnt;
                            foundMatchType = currentMatchType;
                        }
                        else if (foundMatchType == currentMatchType)
                        {
                            throw new AmbiguousMatchException($"Ambiguous match between {evnt.GetFullDescription()} and {foundEventInfo.GetFullDescription()}");
                        }
                    }
                }
            }

            if (foundEventInfo is null && eventIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the type hierarchy to find the method
                foundEventInfo = findEventRecursive(eventIdentifier.StepDownInTypeHierarchy() as EventInfoKey, flags);
            }

            if (!(foundEventInfo is null))
            {
                if (_accessorCache.TryGetValue(eventIdentifier, out MemberInfo cachedMemberInfo))
                {
                    if (cachedMemberInfo != foundEventInfo)
                    {
                        throw new Exception($"EventInfoKey {eventIdentifier} already present in dictionary, but results don't match! Cached: {cachedMemberInfo.GetFullDescription()}, Found: {foundEventInfo.GetFullDescription()}");
                    }
                }
                else
                {
                    _accessorCache.Add(eventIdentifier, foundEventInfo);
                }
            }

            return foundEventInfo;
        }

        static EventInfo findEvent(EventInfoKey eventIdentifier, BindingFlags flags)
        {
            if (_accessorCache.TryGetValue(eventIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is EventInfo cachedEventInfo)
                return cachedEventInfo;

            EventInfo eventInfo = findEventRecursive(eventIdentifier, flags);
            if (eventInfo is null)
                throw new MissingMemberException(eventIdentifier.ReflectedType.FullName, eventIdentifier.MemberName);

            return eventInfo;
        }
        #endregion
        */
    }

}
