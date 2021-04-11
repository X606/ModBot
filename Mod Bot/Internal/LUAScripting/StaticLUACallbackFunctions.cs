using AdvancedColorPicker;
using HarmonyLib;
using ModLibrary;
using MoonSharp.Interpreter;
using PicaVoxel;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UdpKit;
using UnityEngine;
using UnityEngine.Events;

namespace InternalModBot
{
    internal static class StaticLUACallbackFunctions
    {
        static List<GlobalFunction> _globalFunctions;

        public static void AddGlobalFunctions(Script ownerScript)
        {
            if (_globalFunctions == null)
            {
                _globalFunctions = new List<GlobalFunction>();

                foreach (Type type in UserData.GetRegisteredTypes())
                {
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo method in methodInfos)
                    {
                        if (method.IsSpecialName && !method.IsConstructor)
                            continue;

                        if (method.IsDefined(typeof(ObsoleteAttribute)))
                            continue;

                        if (method.IsDefined(typeof(ExtensionAttribute)))
                        {
                            if (!UserData.IsTypeRegistered(method.DeclaringType))
                                UserData.RegisterExtensionType(method.DeclaringType);
                        }
                        else
                        {
                            string key;
                            if (method.IsConstructor)
                            {
                                key = method.DeclaringType.Name;
                            }
                            else
                            {
                                key = method.DeclaringType.Name + "_" + method.Name;
                            }

                            MethodInfo defined = typeof(StaticLUACallbackFunctions).GetMethod(key, BindingFlags.NonPublic | BindingFlags.Static);
                            if (defined != null)
                            {
                                if (ownerScript.Globals.Get(key).IsNil())
                                {
                                    ParameterInfo[] parameters = defined.GetParameters();

                                    List<Type> types = new List<Type>();
                                    foreach (ParameterInfo parameterInfo in parameters)
                                    {
                                        types.Add(parameterInfo.ParameterType);
                                    }
                                    types.Add(defined.ReturnType);

                                    Type delegateType = Expression.GetDelegateType(types.ToArray());
                                    Delegate callback = Delegate.CreateDelegate(delegateType, defined);

                                    _globalFunctions.Add(new GlobalFunction(callback, key));
                                    ownerScript.Globals[key] = callback;

                                    debug.Log("Added " + method.DeclaringType.FullName + "." + method.Name + " with key " + key);
                                }
                            }
                            else
                            {
                                if (ownerScript.Globals.Get(key).IsNil())
                                {
                                    ParameterInfo[] parameters = method.GetParameters();

                                    List<Type> types = new List<Type>();
                                    foreach (ParameterInfo parameterInfo in parameters)
                                    {
                                        types.Add(parameterInfo.ParameterType);
                                    }
                                    types.Add(method.ReturnType);

                                    Type delegateType;
                                    try
                                    {
                                        delegateType = Expression.GetDelegateType(types.ToArray());
                                    }
                                    catch (PlatformNotSupportedException)
                                    {
                                        debug.Log("Not supported: " + method.FullDescription());
                                        continue;
                                    }

                                    Delegate callback;
                                    try
                                    {
                                        callback = Delegate.CreateDelegate(delegateType, method);
                                    }
                                    catch (Exception e)
                                    {
                                        debug.Log(method.FullDescription() + ": " + e.ToString());
                                        continue;
                                    }
                                    _globalFunctions.Add(new GlobalFunction(callback, key));

                                    try
                                    {
                                        ownerScript.Globals[key] = callback;
                                        debug.Log("Added " + method.DeclaringType.FullName + "." + method.Name + " with key " + key);
                                    }
                                    catch (Exception e)
                                    {
                                        if (e.Message == "Method cannot contain unresolved generic parameters")
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            throw e;
                                        }
                                    }
                                }
                                else
                                {
                                    debug.Log("Method name already used! " + method.Name + " (" + key + ")");
                                }
                            }
                        }
                    }
                }

                /*
                MethodInfo[] methods = typeof(StaticLUACallbackFunctions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
                foreach (MethodInfo method in methods)
                {
                    ParameterInfo[] parameters = method.GetParameters();

                    List<Type> types = new List<Type>();
                    foreach (ParameterInfo parameterInfo in parameters)
                    {
                        types.Add(parameterInfo.ParameterType);
                    }
                    types.Add(method.ReturnType);

                    Type delegateType = Expression.GetDelegateType(types.ToArray());

                    Delegate callback = Delegate.CreateDelegate(delegateType, method);
                    _globalFunctions.Add(new GlobalFunction(callback, method.Name));
                    ownerScript.Globals[method.Name] = callback;
                }
                */
            }
            else
            {
                foreach (GlobalFunction globalFunction in _globalFunctions)
                {
                    ownerScript.Globals[globalFunction.Name] = globalFunction.Callback;
                }
            }
        }

        #region BoltLauncher
        internal static void BoltLauncher_StartSinglePlayer(BoltConfig config)
        {
            if (config == null)
                config = BoltRuntimeSettings.instance.GetConfigCopy();

            BoltLauncher.StartSinglePlayer(config);
        }
        internal static void BoltLauncher_StartServer(DynValue first, DynValue second, DynValue third)
        {
            if (first.IsVoid() && second.IsVoid() && third.IsVoid())
            {
                // No arguments
                BoltLauncher.StartServer();
            }
            else if (first.Type == DataType.Number && second.IsVoid() && third.IsVoid())
            {
                // int
                BoltLauncher.StartServer(Convert.ToInt32(first.Number));
            }
            else if (first.Type == DataType.UserData && second.IsVoid() && third.IsVoid())
            {
                // [UserData]

                object firstObj = first.ToObject();
                if (firstObj.GetType() == typeof(BoltConfig))
                {
                    // BoltConfig
                    BoltLauncher.StartServer((BoltConfig)firstObj);
                }
                else if (firstObj.GetType() == typeof(UdpEndPoint))
                {
                    // UdpEndPoint
                    BoltLauncher.StartServer((UdpEndPoint)firstObj);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartServer exists with the signature (" + firstObj.GetType().FullName + ")");
                }
            }
            else if (first.Type == DataType.UserData && second.Type == DataType.String && third.IsVoid())
            {
                // [UserData], string

                object firstObj = first.ToObject();
                if (firstObj.GetType() == typeof(BoltConfig))
                {
                    // BoltConfig, string
                    BoltLauncher.StartServer((BoltConfig)firstObj, second.String);
                }
                else if (firstObj.GetType() == typeof(UdpEndPoint))
                {
                    // UdpEndPoint, string
                    BoltLauncher.StartServer((UdpEndPoint)firstObj, second.String);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartServer exists with the signature (" + firstObj.GetType().FullName + ", string)");
                }
            }
            else if (first.Type == DataType.UserData && second.Type == DataType.UserData && third.IsVoid())
            {
                // [UserData], [UserData]

                object firstObj = first.ToObject();
                object secondObj = second.ToObject();

                if (firstObj.GetType() == typeof(UdpEndPoint) && secondObj.GetType() == typeof(BoltConfig))
                {
                    // UdpEndPoint, BoltConfig
                    BoltLauncher.StartServer((UdpEndPoint)firstObj, (BoltConfig)secondObj);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartServer exists with the signature (" + firstObj.GetType().FullName + ", " + secondObj.GetType().FullName + ")");
                }
            }
            else if (first.Type == DataType.UserData && second.Type == DataType.UserData && third.Type == DataType.String)
            {
                // [UserData], [UserData], string

                object firstObj = first.ToObject();
                object secondObj = second.ToObject();

                if (firstObj.GetType() == typeof(UdpEndPoint) && secondObj.GetType() == typeof(BoltConfig))
                {
                    // UdpEndPoint, BoltConfig, string
                    BoltLauncher.StartServer((UdpEndPoint)firstObj, (BoltConfig)secondObj, second.String);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartServer exists with the signature (" + firstObj.GetType().FullName + ", " + secondObj.GetType().FullName + ", string)");
                }
            }
            else
            {
                List<string> typeNames = new List<string>();
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }
                if (third.IsNotVoid())
                {
                    if (third.Type == DataType.UserData)
                    {
                        typeNames.Add(third.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(third.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for BoltLauncher_StartServer exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        internal static void BoltLauncher_StartClient(DynValue first, DynValue second)
        {
            if (first.IsVoid() && second.IsVoid())
            {
                // No arguments
                BoltLauncher.StartClient();
            }
            else if (first.Type == DataType.Number && second.IsVoid())
            {
                // int
                BoltLauncher.StartClient(Convert.ToInt32(first.Number));
            }
            else if (first.Type == DataType.UserData && second.IsVoid())
            {
                // [UserData]

                object firstObj = first.ToObject();
                if (firstObj.GetType() == typeof(BoltConfig))
                {
                    // BoltConfig
                    BoltLauncher.StartClient((BoltConfig)firstObj);
                }
                else if (firstObj.GetType() == typeof(UdpEndPoint))
                {
                    // UdpEndPoint
                    BoltLauncher.StartClient((UdpEndPoint)firstObj);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartClient exists with the signature (" + firstObj.GetType().FullName + ")");
                }
            }
            else if (first.Type == DataType.UserData && second.Type == DataType.UserData)
            {
                // [UserData], [UserData]

                object firstObj = first.ToObject();
                object secondObj = second.ToObject();
                if (firstObj.GetType() == typeof(UdpEndPoint) && secondObj.GetType() == typeof(BoltConfig))
                {
                    // UdpEndPoint, BoltConfig
                    BoltLauncher.StartClient((UdpEndPoint)firstObj, (BoltConfig)secondObj);
                }
                else
                {
                    throw new ScriptRuntimeException("No overload for BoltLauncher_StartClient exists with the signature (" + firstObj.GetType().FullName + ", " + secondObj.GetType().FullName + ")");
                }
            }
            else
            {
                List<string> typeNames = new List<string>();
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for BoltLauncher_StartClient exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        #endregion

        #region PhotonPoller
        internal static DynValue PhotonPoller_RecvFrom(Script source, byte[] buffer, int bufferSize)
        {
            UdpEndPoint udpEndPoint = new UdpEndPoint();
            int result = PhotonPoller.RecvFrom(buffer, bufferSize, ref udpEndPoint);

            return DynValue.NewTuple(DynValue.NewNumber(result), DynValue.FromObject(source, udpEndPoint));
        }
        #endregion

        #region PlayerInputController
        internal static void PlayerInputController_SetMapsEnabled(DynValue first, DynValue second, DynValue third)
        {
            if (first.Type == DataType.Boolean && second.Type == DataType.Number && third.IsVoid())
            {
                PlayerInputController.SetMapsEnabled(first.Boolean, Convert.ToInt32(second.Number));
            }
            else if (first.Type == DataType.UserData && first.ToObject().GetType() == typeof(Player) && second.Type == DataType.Boolean && third.Type == DataType.Number)
            {
                PlayerInputController.SetMapsEnabled(first.ToObject<Player>(), first.Boolean, Convert.ToInt32(second.Number));
            }
            else
            {
                List<string> typeNames = new List<string>();
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }
                if (third.IsNotVoid())
                {
                    if (third.Type == DataType.UserData)
                    {
                        typeNames.Add(third.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(third.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for PlayerInputController_SetMapsEnabled exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        #endregion

        #region ProfanityChecker
        internal static DynValue ProfanityChecker_FilterForProfanities(string inputString, DynValue onFiltered)
        {
            if (onFiltered.IsVoid())
            {
                return DynValue.NewString(ProfanityChecker.FilterForProfanities(inputString));
            }
            else if (onFiltered.Type == DataType.Function)
            {
                ProfanityChecker.FilterForProfanities(inputString, delegate (string output) { onFiltered.Function.Call(output); });
                return DynValue.Void;
            }
            else
            {
                List<string> typeNames = new List<string>();
                typeNames.Add("string");
                if (onFiltered.IsNotVoid())
                {
                    if (onFiltered.Type == DataType.UserData)
                    {
                        typeNames.Add(onFiltered.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(onFiltered.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for ProfanityChecker_FilterForProfanities exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        #endregion

        #region WorldQueries
        internal static double WorldQueries_GetRotationYFromDirection(DynValue first, DynValue second)
        {
            if (first.Type == DataType.Number && second.Type == DataType.Number)
            {
                return WorldQueries.GetRotationYFromDirection(Convert.ToSingle(first.Number), Convert.ToSingle(second.Number));
            }
            else if (first.Type == DataType.UserData && first.ToObject().GetType() == typeof(Vector3) && second.IsVoid())
            {
                return WorldQueries.GetRotationYFromDirection(first.ToObject<Vector3>());
            }
            else
            {
                List<string> typeNames = new List<string>();
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for WorldQueries_GetRotationYFromDirection exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        #endregion

        #region Helper
        internal static Voxel[] Helper_CopyVoxelsInBox(Voxel[] source, DynValue first, DynValue second, DynValue third, DynValue fourth, DynValue fifth)
        {
            Voxel[] dest = new Voxel[source.Length];

            if (first.Type == DataType.UserData && first.ToObject().GetType() == typeof(PicaVoxelPoint) && second.Type == DataType.UserData && second.ToObject().GetType() == typeof(PicaVoxelPoint) && third.Type == DataType.Boolean && fourth.IsVoid() && fifth.IsVoid())
            {
                Helper.CopyVoxelsInBox(ref source, ref dest, first.ToObject<PicaVoxelPoint>(), second.ToObject<PicaVoxelPoint>(), third.Boolean);
                return dest;
            }
            else if (first.Type == DataType.UserData && first.ToObject().GetType() == typeof(PicaVoxelBox) && second.Type == DataType.UserData && second.ToObject().GetType() == typeof(PicaVoxelBox) && third.Type == DataType.UserData && third.ToObject().GetType() == typeof(PicaVoxelPoint) && fourth.Type == DataType.UserData && fourth.ToObject().GetType() == typeof(PicaVoxelPoint) && fifth.Type == DataType.Boolean)
            {
                Helper.CopyVoxelsInBox(ref source, ref dest, first.ToObject<PicaVoxelBox>(), second.ToObject<PicaVoxelBox>(), third.ToObject<PicaVoxelPoint>(), fourth.ToObject<PicaVoxelPoint>(), third.Boolean);
                return dest;
            }
            else
            {
                List<string> typeNames = new List<string>();
                typeNames.Add("Voxel[]");
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }
                if (third.IsNotVoid())
                {
                    if (third.Type == DataType.UserData)
                    {
                        typeNames.Add(third.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(third.Type.ToString());
                    }
                }
                if (fourth.IsNotVoid())
                {
                    if (fourth.Type == DataType.UserData)
                    {
                        typeNames.Add(fourth.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(fourth.Type.ToString());
                    }
                }
                if (fifth.IsNotVoid())
                {
                    if (fifth.Type == DataType.UserData)
                    {
                        typeNames.Add(fifth.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(fifth.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for Helper_CopyVoxelsInBox exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        internal static Voxel[] Helper_RotateVoxelArrayX(Voxel[] source, PicaVoxelPoint srcSize)
        {
            Voxel[] result = new Voxel[source.Length];
            source.CopyTo(result, 0);

            Helper.RotateVoxelArrayX(ref result, srcSize);
            return result;
        }
        internal static Voxel[] Helper_RotateVoxelArrayY(Voxel[] source, PicaVoxelPoint srcSize)
        {
            Voxel[] result = new Voxel[source.Length];
            source.CopyTo(result, 0);

            Helper.RotateVoxelArrayY(ref result, srcSize);
            return result;
        }
        internal static Voxel[] Helper_RotateVoxelArrayZ(Voxel[] source, PicaVoxelPoint srcSize)
        {
            Voxel[] result = new Voxel[source.Length];
            source.CopyTo(result, 0);

            Helper.RotateVoxelArrayZ(ref result, srcSize);
            return result;
        }
        #endregion

        #region MeshGenerator
        internal static DynValue MeshGenerator_GenerateGreedy(Script source, List<Vector3> vertices, List<Vector2> uvs, List<Color32> colors, List<int> indexes, Voxel[] invoxels, float voxelSize, float overlapAmount, int xOffset, int yOffset, int zOffset, int xSize, int ySize, int zSize, int ub0, int ub1, int ub2, float selfShadeIntensity)
        {
            List<Vector3> verticesCopy = new List<Vector3>(vertices);
            List<Vector2> uvsCopy = new List<Vector2>(uvs);
            List<Color32> colorsCopy = new List<Color32>(colors);
            List<int> indexesCopy = new List<int>(indexes);

            Voxel[] invoxelsCopy = new Voxel[invoxels.Length];
            invoxels.CopyTo(invoxelsCopy, 0);

            Vector3[] vertArray = new Vector3[0];
            Vector2[] uvArray = new Vector2[0];
            Color32[] colorArray = new Color32[0];
            int[] indexArray = new int[0];

            MeshGenerator.GenerateGreedy(verticesCopy, uvsCopy, colorsCopy, indexesCopy, ref vertArray, ref uvArray, ref colorArray, ref indexArray, ref invoxelsCopy, voxelSize, overlapAmount, xOffset, yOffset, zOffset, xSize, ySize, zSize, ub0, ub1, ub2, selfShadeIntensity);

            return DynValue.NewTuple(DynValue.FromObject(source, vertArray), DynValue.FromObject(source, uvArray), DynValue.FromObject(source, colorArray), DynValue.FromObject(source, indexArray));
        }
        internal static DynValue MeshGenerator_GenerateCulled(Script source, List<Vector3> vertices, List<Vector2> uvs, List<Color32> colors, List<int> indexes, Voxel[] invoxels, float voxelSize, float overlapAmount, int xOffset, int yOffset, int zOffset, int xSize, int ySize, int zSize, int ub0, int ub1, int ub2, float selfShadeIntensity)
        {
            List<Vector3> verticesCopy = new List<Vector3>(vertices);
            List<Vector2> uvsCopy = new List<Vector2>(uvs);
            List<Color32> colorsCopy = new List<Color32>(colors);
            List<int> indexesCopy = new List<int>(indexes);

            Voxel[] invoxelsCopy = new Voxel[invoxels.Length];
            invoxels.CopyTo(invoxelsCopy, 0);

            Vector3[] vertArray = new Vector3[0];
            Vector2[] uvArray = new Vector2[0];
            Color32[] colorArray = new Color32[0];
            int[] indexArray = new int[0];

            MeshGenerator.GenerateCulled(verticesCopy, uvsCopy, colorsCopy, indexesCopy, ref vertArray, ref uvArray, ref colorArray, ref indexArray, ref invoxelsCopy, voxelSize, overlapAmount, xOffset, yOffset, zOffset, xSize, ySize, zSize, ub0, ub1, ub2, selfShadeIntensity);

            return DynValue.NewTuple(DynValue.FromObject(source, vertArray), DynValue.FromObject(source, uvArray), DynValue.FromObject(source, colorArray), DynValue.FromObject(source, indexArray));
        }
        internal static DynValue MeshGenerator_GenerateMarching(Script source, List<Vector3> vertices, List<Vector2> uvs, List<Color32> colors, List<int> indexes, Voxel[] invoxels, float voxelSize, int xOffset, int yOffset, int zOffset, int xSize, int ySize, int zSize, int ub0, int ub1, int ub2, float selfShadeIntensity)
        {
            List<Vector3> verticesCopy = new List<Vector3>(vertices);
            List<Vector2> uvsCopy = new List<Vector2>(uvs);
            List<Color32> colorsCopy = new List<Color32>(colors);
            List<int> indexesCopy = new List<int>(indexes);

            Voxel[] invoxelsCopy = new Voxel[invoxels.Length];
            invoxels.CopyTo(invoxelsCopy, 0);

            Vector3[] vertArray = new Vector3[0];
            Vector2[] uvArray = new Vector2[0];
            Color32[] colorArray = new Color32[0];
            int[] indexArray = new int[0];

            MeshGenerator.GenerateMarching(verticesCopy, uvsCopy, colorsCopy, indexesCopy, ref vertArray, ref uvArray, ref colorArray, ref indexArray, ref invoxelsCopy, voxelSize, xOffset, yOffset, zOffset, xSize, ySize, zSize, ub0, ub1, ub2, selfShadeIntensity);

            return DynValue.NewTuple(DynValue.FromObject(source, vertArray), DynValue.FromObject(source, uvArray), DynValue.FromObject(source, colorArray), DynValue.FromObject(source, indexArray));
        }
        #endregion

        #region GraphicalColorTypeComponent
        internal static DynValue GraphicalColorTypeComponent_CalculateNewValue(ColorType newValue, ColorType value1, ColorType value2, ColorType value3)
        {
            bool result = GraphicalColorTypeComponent.CalculateNewValue(newValue, ref value1, ref value2, ref value3);
            return DynValue.NewTuple(DynValue.NewBoolean(result), DynValue.NewNumber((int)value1), DynValue.NewNumber((int)value2), DynValue.NewNumber((int)value3));
        }
        #endregion

        #region InputBlocker
        internal static DynValue InputBlocker_CreateBlocker(Script source, DynValue first, DynValue second, DynValue third)
        {
            if (first.Type == DataType.Function && second.IsVoid() && third.IsVoid())
            {
                InputBlocker.CreateBlocker(delegate { first.Function.Call(); });
                return DynValue.Void;
            }
            else if (first.Type == DataType.Function && second.Type == DataType.Number && third.IsVoid())
            {
                InputBlocker.CreateBlocker(delegate { first.Function.Call(); }, Convert.ToInt32(second.Number));
                return DynValue.Void;
            }
            else if (first.Type == DataType.Function && second.Type == DataType.Number && third.Type == DataType.Number)
            {
                InputBlocker.CreateBlocker(delegate { first.Function.Call(); }, Convert.ToInt32(second.Number), Convert.ToInt32(third.Number));
                return DynValue.Void;
            }
            else if (first.Type == DataType.UserData && first.ToObject().GetType() == typeof(RectTransform) && second.Type == DataType.Function && third.IsVoid())
            {
                return DynValue.FromObject(source, InputBlocker.CreateBlocker(first.ToObject<RectTransform>(), delegate { first.Function.Call(); }));
            }
            else
            {
                List<string> typeNames = new List<string>();
                if (first.IsNotVoid())
                {
                    if (first.Type == DataType.UserData)
                    {
                        typeNames.Add(first.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(first.Type.ToString());
                    }
                }
                if (second.IsNotVoid())
                {
                    if (second.Type == DataType.UserData)
                    {
                        typeNames.Add(second.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(second.Type.ToString());
                    }
                }
                if (third.IsNotVoid())
                {
                    if (third.Type == DataType.UserData)
                    {
                        typeNames.Add(third.ToObject().GetType().FullName);
                    }
                    else
                    {
                        typeNames.Add(third.Type.ToString());
                    }
                }

                throw new ScriptRuntimeException("No overload for InputBlocker_CreateBlocker exists with the signature (" + typeNames.Join(", ") + ")");
            }
        }
        #endregion




        #region Color
        internal static LUAColor Color(double red, double green, double blue, DynValue alpha)
        {
            return new LUAColor
            {
                r = red,
                g = green,
                b = blue,
                a = alpha.IsNotNil() ? alpha.Number : 1d
            };
        }
        internal static LUAColor ColorLerp(LUAColor a, LUAColor b, double t)
        {
            t = Clamp01(t);
            return new LUAColor(a.r + ((b.r - a.r) * t), a.g + ((b.g - a.g) * t), a.b + ((b.b - a.b) * t), a.a + ((b.a - a.a) * t));
        }
        internal static LUAColor ColorLerpUnclamped(LUAColor a, LUAColor b, double t)
        {
            return new LUAColor(a.r + ((b.r - a.r) * t), a.g + ((b.g - a.g) * t), a.b + ((b.b - a.b) * t), a.a + ((b.a - a.a) * t));
        }
        #endregion

        #region Vec2
        internal static Vec2 Vec2()
        {
            return new Vec2();
        }
        internal static Vec2 Vec2(double x, double y)
        {
            return new Vec2(x, y);
        }
        internal static Vec2 Vec2Lerp(Vec2 a, Vec2 b, double t)
        {
            t = Clamp01(t);
            return new Vec2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));
        }
        internal static Vec2 Vec2LerpUnclamped(Vec2 a, Vec2 b, double t)
        {
            return new Vec2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));
        }
        internal static Vec2 Vec2MoveTowards(Vec2 current, Vec2 target, double maxDistanceDelta)
        {
            Vec2 a = target - current;
            double magnitude = a.magnitude;
            Vec2 result;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                result = target;
            }
            else
            {
                result = current + (a / magnitude * maxDistanceDelta);
            }
            return result;
        }
        internal static Vec2 Vec2Reflect(Vec2 inDirection, Vec2 inNormal)
        {
            return (-2d * Vec2Dot(inNormal, inDirection) * inNormal) + inDirection;
        }
        internal static Vec2 Vec2Perpendicular(Vec2 inDirection)
        {
            return new Vec2(-inDirection.y, inDirection.x);
        }
        internal static double Vec2Dot(Vec2 lhs, Vec2 rhs)
        {
            return (lhs.x * rhs.x) + (lhs.y * rhs.y);
        }
        internal static double Vec2Angle(Vec2 from, Vec2 to)
        {
            double num = Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            double result;
            if (num < 1E-15f)
            {
                result = 0f;
            }
            else
            {
                double f = Clamp(Vec2Dot(from, to) / num, -1f, 1f);
                result = Math.Acos(f) * 57.29578f;
            }
            return result;
        }
        internal static double Vec2SignedAngle(Vec2 from, Vec2 to)
        {
            double num = Vec2Angle(from, to);
            double num2 = Math.Sign((from.x * to.y) - (from.y * to.x));
            return num * num2;
        }
        internal static double Vec2Distance(Vec2 a, Vec2 b)
        {
            return (a - b).magnitude;
        }
        internal static Vec2 Vec2ClampMagnitude(Vec2 vector, double maxLength)
        {
            Vec2 result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }
        internal static Vec2 Vec2Min(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }
        internal static Vec2 Vec2Max(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }
        internal static DynValue Vec2SmoothDamp(Script source,Vec2 current, Vec2 target, Vec2 currentVelocity, double smoothTime, double maxSpeed)
        {
            double deltaTime = Time.deltaTime;
            return Vec2SmoothDamp(source, current, target, currentVelocity, smoothTime, maxSpeed, deltaTime);
        }
        internal static DynValue Vec2SmoothDamp(Script source, Vec2 current, Vec2 target, Vec2 currentVelocity, double smoothTime)
        {
            double deltaTime = Time.deltaTime;
            double positiveInfinity = double.PositiveInfinity;
            return Vec2SmoothDamp(source, current, target, currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }
        internal static DynValue Vec2SmoothDamp(Script source, Vec2 current, Vec2 target, Vec2 currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            double num = 2f / smoothTime;
            double num2 = num * deltaTime;
            double d = 1f / (1f + num2 + (0.48f * num2 * num2) + (0.235f * num2 * num2 * num2));
            Vec2 vector = current - target;
            Vec2 vector2 = target;
            double maxLength = maxSpeed * smoothTime;
            vector = Vec2ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vec2 vector3 = (currentVelocity + (num * vector)) * deltaTime;
            Vec2 currentVelocityOut = (currentVelocity - (num * vector3)) * d;
            Vec2 vector4 = target + ((vector + vector3) * d);
            if (Vec2Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocityOut = (vector4 - vector2) / deltaTime;
            }

            return DynValue.NewTuple(DynValue.FromObject(source, vector4), DynValue.FromObject(source, currentVelocityOut));
        }
        #endregion

        #region Vec3
        internal static Vec3 Vec3()
        {
            return new Vec3();
        }
        internal static Vec3 Vec3(double x, double y, double z)
        {
            return new Vec3(x, y, z);
        }
        internal static Vec3 Vec3Slerp(Vec3 a, Vec3 b, float t)
        {
            return Vector3.Slerp(a, b, t);
        }
        internal static Vec3 Vec3SlerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return Vector3.SlerpUnclamped(a, b, t);
        }
        internal static DynValue Vec3OrthoNormalize(Script source, Vec3 normal, Vec3 tangent, Vec3 binormal)
        {
            Vector3 vector3Normal = normal;
            Vector3 vector3tangent = tangent;

            if (binormal == null)
            {
                Vector3.OrthoNormalize(ref vector3Normal, ref vector3tangent);
                return DynValue.NewTuple(DynValue.FromObject(source, (Vec3)vector3Normal), DynValue.FromObject(source, (Vec3)vector3tangent));
            }
            else
            {
                Vector3 vector3binormal = binormal;

                Vector3.OrthoNormalize(ref vector3Normal, ref vector3tangent, ref vector3binormal);
                return DynValue.NewTuple(DynValue.FromObject(source, (Vec3)vector3Normal), DynValue.FromObject(source, (Vec3)vector3tangent), DynValue.FromObject(source, (Vec3)vector3binormal));
            }
        }
        internal static Vec3 Vec3RotateTowards(Vec3 current, Vec3 target, float maxRadiansDelta, float maxMagnitudeDelta)
        {
            return Vector3.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta);
        }
        internal static Vec3 Vec3Lerp(Vec3 a, Vec3 b, double t)
        {
            t = Clamp01(t);
            return new Vec3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
        }
        internal static Vec3 Vec3LerpUnclamped(Vec3 a, Vec3 b, double t)
        {
            return new Vec3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
        }
        internal static Vec3 Vec3MoveTowards(Vec3 current, Vec3 target, double maxDistanceDelta)
        {
            Vec3 a = target - current;
            double magnitude = a.magnitude;
            Vec3 result;
            if (magnitude <= maxDistanceDelta || magnitude < 1E-45f)
            {
                result = target;
            }
            else
            {
                result = current + (a / magnitude * maxDistanceDelta);
            }
            return result;
        }
        internal static DynValue Vec3SmoothDamp(Script source, Vec3 current, Vec3 target, Vec3 currentVelocity, double smoothTime, double maxSpeed)
        {
            double deltaTime = Time.deltaTime;
            return Vec3SmoothDamp(source, current, target, currentVelocity, smoothTime, maxSpeed, deltaTime);
        }
        internal static DynValue Vec3SmoothDamp(Script source, Vec3 current, Vec3 target, Vec3 currentVelocity, double smoothTime)
        {
            double deltaTime = Time.deltaTime;
            double positiveInfinity = double.PositiveInfinity;
            return Vec3SmoothDamp(source, current, target, currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }
        internal static DynValue Vec3SmoothDamp(Script source, Vec3 current, Vec3 target, Vec3 currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Math.Max(0.0001d, smoothTime);
            double num = 2d / smoothTime;
            double num2 = num * deltaTime;
            double d = 1d / (1d + num2 + (0.48d * num2 * num2) + (0.235d * num2 * num2 * num2));
            Vec3 vector = current - target;
            Vec3 vector2 = target;
            double maxLength = maxSpeed * smoothTime;
            vector = Vec3ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vec3 vector3 = (currentVelocity + (num * vector)) * deltaTime;
            Vec3 currentVelocityOut = (currentVelocity - (num * vector3)) * d;
            Vec3 vector4 = target + ((vector + vector3) * d);
            if (Vec3Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocityOut = (vector4 - vector2) / deltaTime;
            }

            return DynValue.NewTuple(DynValue.FromObject(source, vector4), DynValue.FromObject(source, currentVelocityOut));
        }
        internal static Vec3 Vec3Scale(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        internal static Vec3 Vec3Cross(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
        }
        internal static Vec3 Vec3Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            return (-2f * Vec3Dot(inNormal, inDirection) * inNormal) + inDirection;
        }
        internal static double Vec3Dot(Vec3 lhs, Vec3 rhs)
        {
            return (lhs.x * rhs.x) + (lhs.y * rhs.y) + (lhs.z * rhs.z);
        }
        internal static Vec3 Vec3Project(Vec3 vector, Vec3 onNormal)
        {
            double num = Vec3Dot(onNormal, onNormal);
            Vec3 result;
            if (num < Mathf.Epsilon)
            {
                result = new Vec3(0, 0, 0);
            }
            else
            {
                result = onNormal * Vec3Dot(vector, onNormal) / num;
            }
            return result;
        }
        internal static Vec3 Vec3ProjectOnPlane(Vec3 vector, Vec3 planeNormal)
        {
            return vector - Vec3Project(vector, planeNormal);
        }
        internal static double Vec3Angle(Vec3 from, Vec3 to)
        {
            double num = Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            double result;
            if (num < 1E-15f)
            {
                result = 0f;
            }
            else
            {
                double f = Clamp(Vec3Dot(from, to) / num, -1f, 1f);
                result = Math.Acos(f) * 57.29578f;
            }
            return result;
        }
        internal static double Vec3SignedAngle(Vec3 from, Vec3 to, Vec3 axis)
        {
            double num = Vec3Angle(from, to);
            double num2 = Math.Sign(Vec3Dot(axis, Vec3Cross(from, to)));
            return num * num2;
        }
        internal static double Vec3Distance(Vec3 a, Vec3 b)
        {
            Vec3 vector = a - b;
            return Math.Sqrt((vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z));
        }
        internal static Vec3 Vec3ClampMagnitude(Vec3 vector, double maxLength)
        {
            Vec3 result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }
        internal static Vec3 Vec3Min(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }
        internal static Vec3 Vec3Max(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }
        #endregion

        #region Vec4
        internal static Vec4 Vec4()
        {
            return new Vec4();
        }
        internal static Vec4 Vec4(double x, double y, double z, double w)
        {
            return new Vec4(x, y, z, w);
        }
        internal static Vec4 Vec4Lerp(Vec4 a, Vec4 b, double t)
        {
            t = Clamp01(t);
            return new Vec4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));
        }
        internal static Vec4 Vec4LerpUnclamped(Vec4 a, Vec4 b, double t)
        {
            return new Vec4(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t), a.w + ((b.w - a.w) * t));
        }
        internal static Vec4 Vec4MoveTowards(Vec4 current, Vec4 target, double maxDistanceDelta)
        {
            Vec4 a = target - current;
            double magnitude = a.magnitude;
            Vec4 result;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                result = target;
            }
            else
            {
                result = current + (a / magnitude * maxDistanceDelta);
            }
            return result;
        }
        internal static double Vec4Dot(Vec4 a, Vec4 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
        }
        internal static Vec4 Vec4Project(Vec4 a, Vec4 b)
        {
            return b * Vec4Dot(a, b) / Vec4Dot(b, b);
        }
        internal static double Vec4Distance(Vec4 a, Vec4 b)
        {
            return (a - b).magnitude;
        }
        internal static Vec4 Vec4Min(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));
        }
        internal static Vec4 Vec4Max(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));
        }
        #endregion

        #region Quat
        internal static Quat QuatIdentity()
        {
            return new Quat(0d, 0d, 0d, 1d);
        }
        internal static Quat Quat(double x, double y, double z, double w)
        {
            return new Quat(x, y, z, w);
        }
        internal static Quat QuatEuler(float x, float y, float z)
        {
            return Quaternion.Euler(x, y, z);
        }
        internal static Quat QuatEuler(Vec3 euler)
        {
            return Quaternion.Euler(euler);
        }
        internal static Quat QuatFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            return Quaternion.FromToRotation(fromDirection, toDirection);
        }
        internal static Quat QuatInverse(Quat rotation)
        {
            return Quaternion.Inverse(rotation);
        }
        internal static Quat QuatSlerp(Quat a, Quat b, double t)
        {
            return Quaternion.Slerp(a, b, Convert.ToSingle(t));
        }
        internal static Quat QuatSlerpUnclamped(Quat a, Quat b, double t)
        {
            return Quaternion.SlerpUnclamped(a, b, Convert.ToSingle(t));
        }
        internal static Quat QuatLerp(Quat a, Quat b, double t)
        {
            return Quaternion.Lerp(a, b, Convert.ToSingle(t));
        }
        internal static Quat QuatLerpUnclamped(Quat a, Quat b, double t)
        {
            return Quaternion.LerpUnclamped(a, b, Convert.ToSingle(t));
        }
        internal static Quat QuatAngleAxis(double angle, Vec3 axis)
        {
            return Quaternion.AngleAxis(Convert.ToSingle(angle), axis);
        }
        internal static Quat QuatLookRotation(Vec3 forward, Vec3 upwards)
        {
            if (upwards == null)
                upwards = new Vec3(0, 1, 0); // Vec3.up

            return Quaternion.LookRotation(forward, upwards);
        }
        internal static double QuatDot(Quat a, Quat b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
        }
        internal static double QuatAngle(Quat a, Quat b)
        {
            double num = QuatDot(a, b);
            return (!InternalModBot.Quat.IsConsideredEqual(num)) ? (Math.Acos(Math.Min(Math.Abs(num), 1d)) * 2d * 57.29578d) : 0d;
        }
        internal static Quat QuatRotateTowards(Quat from, Quat to, double maxDegreesDelta)
        {
            double num = QuatAngle(from, to);
            Quat result;
            if (num == 0f)
            {
                result = to;
            }
            else
            {
                result = QuatSlerpUnclamped(from, to, Math.Min(1f, maxDegreesDelta / num));
            }
            return result;
        }
        #endregion

        #region Mat4x4
        internal static Mat4x4 Mat4x4()
        {
            return new Mat4x4();
        }
        internal static Mat4x4 Mat4x4(Vec4 column0, Vec4 column1, Vec4 column2, Vec4 column3)
        {
            return new Mat4x4(column0, column1, column2, column3);
        }
        internal static Mat4x4 Mat4x4TRS(Vec3 pos, Quat q, Vec3 s)
        {
            return Matrix4x4.TRS(pos, q, s);
        }
        internal static Mat4x4 Mat4x4Ortho(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            return Matrix4x4.Ortho(left, right, bottom, top, zNear, zFar);
        }
        internal static Mat4x4 Mat4x4Perspective(float fov, float aspect, float zNear, float zFar)
        {
            return Matrix4x4.Perspective(fov, aspect, zNear, zFar);
        }
        internal static Mat4x4 Mat4x4LookAt(Vec3 from, Vec3 to, Vec3 up)
        {
            return Matrix4x4.LookAt(from, to, up);
        }
        internal static Mat4x4 Mat4x4Frustum(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            return Matrix4x4.Frustum(left, right, bottom, top, zNear, zFar);
        }
        internal static Mat4x4 Mat4x4Frustum(FrustumPlanes frustumPlanes)
        {
            return Matrix4x4.Frustum(frustumPlanes);
        }
        internal static Mat4x4 Mat4x4Scale(Vec3 vector)
        {
            return new Mat4x4
            {
                m00 = vector.x,
                m01 = 0d,
                m02 = 0d,
                m03 = 0d,
                m10 = 0d,
                m11 = vector.y,
                m12 = 0d,
                m13 = 0d,
                m20 = 0d,
                m21 = 0d,
                m22 = vector.z,
                m23 = 0d,
                m30 = 0d,
                m31 = 0d,
                m32 = 0d,
                m33 = 1d
            };
        }
        internal static Mat4x4 Mat4x4Translate(Vec3 vector)
        {
            return new Mat4x4
            {
                m00 = 1d,
                m01 = 0d,
                m02 = 0d,
                m03 = vector.x,
                m10 = 0d,
                m11 = 1d,
                m12 = 0d,
                m13 = vector.y,
                m20 = 0d,
                m21 = 0d,
                m22 = 1d,
                m23 = vector.z,
                m30 = 0d,
                m31 = 0d,
                m32 = 0d,
                m33 = 1d
            };
        }
        internal static Mat4x4 Mat4x4Rotate(Quat q)
        {
            double num = q.x * 2d;
            double num2 = q.y * 2d;
            double num3 = q.z * 2d;
            double num4 = q.x * num;
            double num5 = q.y * num2;
            double num6 = q.z * num3;
            double num7 = q.x * num2;
            double num8 = q.x * num3;
            double num9 = q.y * num3;
            double num10 = q.w * num;
            double num11 = q.w * num2;
            double num12 = q.w * num3;

            return new Mat4x4
            {
                m00 = 1d - (num5 + num6),
                m10 = num7 + num12,
                m20 = num8 - num11,
                m30 = 0d,
                m01 = num7 - num12,
                m11 = 1d - (num4 + num6),
                m21 = num9 + num10,
                m31 = 0d,
                m02 = num8 + num11,
                m12 = num9 - num10,
                m22 = 1d - (num4 + num5),
                m32 = 0d,
                m03 = 0d,
                m13 = 0d,
                m23 = 0d,
                m33 = 1d
            };
        }
        #endregion

        #region FrustumPlanes
        internal static FrustumPlanes FrustumPlanes()
        {
            return new FrustumPlanes();
        }
        internal static FrustumPlanes FrustumPlanes(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            return new FrustumPlanes { left = left, right = right, bottom = bottom, top = top, zNear = zNear, zFar = zFar };
        }
        #endregion

        #region Plane
        internal static Plane Plane()
        {
            return new Plane();
        }
        internal static Plane Plane(Vec3 inNormal, Vec3 inPoint)
        {
            return new Plane(inNormal, inPoint);
        }
        internal static Plane Plane(Vec3 inNormal, float d)
        {
            return new Plane(inNormal, d);
        }
        internal static Plane Plane(Vec3 a, Vec3 b, Vec3 c)
        {
            return new Plane(a, b, c);
        }
        #endregion

        #region GameObject
        internal static GameObject GameObjectCreateEmpty(string name)
        {
            return new GameObject(name);
        }
        internal static GameObject GameObjectCreatePrimitive(string type)
        {
            if (Enum.TryParse(type, true, out PrimitiveType primitiveType))
            {
                return GameObject.CreatePrimitive(primitiveType);
            }
            else
            {
                throw ScriptRuntimeException.BadArgumentIndexOutOfRange(nameof(GameObjectCreatePrimitive), 0);
            }
        }
        internal static GameObject FindGameObjectWithTag(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        internal static GameObject[] FindGameObjectsWithTag(string tag)
        {
            return GameObject.FindGameObjectsWithTag(tag);
        }
        internal static GameObject FindGameObject(string name)
        {
            return GameObject.Find(name);
        }
        #endregion

        #region Math
        internal static double Clamp01(double value)
        {
            return Clamp(value, 0f, 1f);
        }
        internal static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }
        #endregion

        #region Debug
        internal static void DebugPrint(DynValue message, LUAColor textColor)
        {
            if (message.Type == DataType.String)
            {
                debug.Log(message.String, textColor ?? UnityEngine.Color.white); // If we don't do this, strings will be printed with the double quotes included for some reason
            }
            else
            {
                debug.Log(message, textColor ?? UnityEngine.Color.white);
            }
        }
        internal static void DebugLine(Vec3 start, Vec3 end, LUAColor lineColor, DynValue timeToStay)
        {
            float timeToStayNum = 0f;
            if (timeToStay != null && timeToStay.Type == DataType.Number)
                timeToStayNum = Convert.ToSingle(timeToStay.Number);

            debug.DrawLine(start, end, lineColor ?? UnityEngine.Color.red, timeToStayNum);
        }
        internal static void DebugRay(Vec3 origin, Vec3 rayDir, LUAColor lineColor, DynValue timeToStay)
        {
            float timeToStayNum = 0f;
            if (timeToStay != null && timeToStay.Type == DataType.Number)
                timeToStayNum = Convert.ToSingle(timeToStay.Number);

            debug.DrawRay(origin, rayDir, lineColor ?? UnityEngine.Color.red, timeToStayNum);
        }
        #endregion

        class GlobalFunction
        {
            public Delegate Callback;
            public string Name;

            public GlobalFunction(Delegate callback, string name)
            {
                Callback = callback;
                Name = name;
            }
        }
    }
}
