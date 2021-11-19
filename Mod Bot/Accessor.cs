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

        static ParameterMatchType parameterTypesMatch(ParameterInfo[] parameters, MatchType[] argumentTypes)
        {
            if (argumentTypes == null)
                argumentTypes = new MatchType[0];

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

                Type parameterType = parameters[i].ParameterType;

                // Null argument: any type goes, as long as it's nullable
                if (argumentTypes[i] == null)
                {
                    // If the parameter type is a value type, and it's not a Nullable<T>, it cannot be null and thus doesn't match
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                        return ParameterMatchType.NoMatch;
                }
                else if (!argumentTypes[i].IsAssignableTo(parameterType))
                {
                    return ParameterMatchType.NoMatch;
                }
            }

            return ParameterMatchType.FullMatch;
        }

        #region Method
        static MethodInfo findMethodRecursive(MethodInfoKey methodIdentifier)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            MethodInfo foundMethodInfo = null;

            MethodInfo[] methodInfos = methodIdentifier.ReflectedType.GetMethods(FLAGS);
            foreach (MethodInfo method in methodInfos)
            {
                if (method.Name == methodIdentifier.MemberName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    ParameterMatchType currentMatchType = parameterTypesMatch(parameters, methodIdentifier.ParameterTypes);
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
                foundMethodInfo = findMethodRecursive(methodIdentifier.StepDownInTypeHierarchy() as MethodInfoKey);
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

        static MethodInfo findMethod(MethodInfoKey methodIdentifier)
        {
            if (_accessorCache.TryGetValue(methodIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is MethodInfo cachedMethodInfo)
                return cachedMethodInfo;

            MethodInfo methodInfo = findMethodRecursive(methodIdentifier);
            if (methodInfo is null)
                throw new MissingMethodException(methodIdentifier.ReflectedType.FullName, methodIdentifier.MemberName);

            return methodInfo;
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

            return (ReturnType)findMethod(new MethodInfoKey(typeof(InstanceType), methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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

            return (ReturnType)findMethod(new MethodInfoKey(instance.GetType(), methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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

            return (ReturnType)findMethod(new MethodInfoKey(declaringType, methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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

            findMethod(new MethodInfoKey(typeof(InstanceType), methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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

            findMethod(new MethodInfoKey(instance.GetType(), methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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

            findMethod(new MethodInfoKey(declaringType, methodName, args, parameterTypeOverrides)).Invoke(instance, args);
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
        static FieldInfo findFieldRecursive(FieldInfoKey fieldIdentifier)
        {
            FieldInfo fieldInfo = fieldIdentifier.ReflectedType.GetField(fieldIdentifier.MemberName, FLAGS);

            if (fieldInfo is null && fieldIdentifier.CanStepDownInTypeHierarchy)
            {
                // Recursively go down the type hierarchy to find the field
                fieldInfo = findFieldRecursive(fieldIdentifier.StepDownInTypeHierarchy() as FieldInfoKey);
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
        static FieldInfo findField(FieldInfoKey fieldIdentifier)
        {
            if (_accessorCache.TryGetValue(fieldIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is FieldInfo cachedFieldInfo)
                return cachedFieldInfo;

            FieldInfo fieldInfo = findFieldRecursive(fieldIdentifier);
            if (Equals(fieldInfo, null))
                throw new MissingFieldException(fieldIdentifier.ReflectedType.FullName, fieldIdentifier.MemberName);

            return fieldInfo;
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

            findField(new FieldInfoKey(declaringType, fieldName)).SetValue(instance, value);
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

            findField(new FieldInfoKey(typeof(InstanceType), fieldName)).SetValue(instance, value);
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

            findField(new FieldInfoKey(instance.GetType(), fieldName)).SetValue(instance, value);
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

            return (FieldType)findField(new FieldInfoKey(typeof(InstanceType), fieldName)).GetValue(instance);
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

            return (FieldType)findField(new FieldInfoKey(instance.GetType(), fieldName)).GetValue(instance);
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

            return (FieldType)findField(new FieldInfoKey(declaringType, fieldName)).GetValue(instance);
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
        static PropertyInfo findPropertyRecursive(PropertyInfoKey propertyIdentifier)
        {
            ParameterMatchType foundMatchType = ParameterMatchType.None;
            PropertyInfo foundPropertyInfo = null;

            PropertyInfo[] propertyInfos = propertyIdentifier.ReflectedType.GetProperties(FLAGS);
            foreach (PropertyInfo property in propertyInfos)
            {
                if (property.Name == propertyIdentifier.MemberName)
                {
                    if (propertyIdentifier.ReturnType == null || propertyIdentifier.ReturnType.IsAssignableFrom(property.PropertyType))
                    {
                        ParameterInfo[] parameters = property.GetIndexParameters();

                        ParameterMatchType currentMatchType = parameterTypesMatch(parameters, propertyIdentifier.IndexTypes);
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
                foundPropertyInfo = findPropertyRecursive(propertyIdentifier.StepDownInTypeHierarchy() as PropertyInfoKey);
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
        static PropertyInfo findProperty(PropertyInfoKey propertyIdentifier)
        {
            if (_accessorCache.TryGetValue(propertyIdentifier, out MemberInfo cachedMemberInfo) && cachedMemberInfo is PropertyInfo cachedPropertyInfo)
                return cachedPropertyInfo;

            PropertyInfo propertyInfo = findPropertyRecursive(propertyIdentifier);
            if (Equals(propertyInfo, null))
                throw new MissingMemberException(propertyIdentifier.ReflectedType.FullName, propertyIdentifier.MemberName);

            return propertyInfo;
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

            findProperty(new PropertyInfoKey(declaringType, propertyName, value?.GetType(), indices, parameterTypeOverrides)).SetValue(instance, value);
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

            return findProperty(new PropertyInfoKey(declaringType, propertyName, returnType, indices, parameterTypeOverrides)).GetValue(instance, indices);
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
    }
}
