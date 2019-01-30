using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
//using Harmony;
//using Mono.Cecil;
namespace ModLibrary
{

    //NOTE: None of this works, feel free to try to fix it if you are up to it, but I have tried way too much at this point and I just cant get it to work /X606

    /*
    public static class Injector
    {

        public static void inject(Type typeToReplace, string ReplaceMeathodName, Type typeToInjectWith, string InjectMeathodName, WhenToRun whenToRun)
        {
            HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("com.company.project.product");
            
            var original = typeToReplace.GetMethod(ReplaceMeathodName);
            MethodInfo prefix = null;
            MethodInfo postfix = null;
            if (whenToRun == WhenToRun.After)
            {
                postfix = typeToInjectWith.GetMethod(InjectMeathodName);
            } else if (whenToRun == WhenToRun.Before)
            {
                prefix = typeToInjectWith.GetMethod(InjectMeathodName);
            }
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        }

        public static void Initialize()
        {
            InjectionHelper.Initialize();
        }
        public unsafe static void inject(Type typeToReplace, string ReplaceMeathodName, Type typeToInjectWith, string InjectMeathodName)
        {
            MethodInfo ReplaceMethod = typeToReplace.GetMethod(ReplaceMeathodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            byte[] IlBytesForReplaceMethod = ReplaceMethod.GetMethodBody().GetILAsByteArray();
            for (int i = 0; i < IlBytesForReplaceMethod.Length; i++)
            {
                Console.WriteLine(IlBytesForReplaceMethod[i]);
            }
            MethodInfo InjectMethod = typeToInjectWith.GetMethod(InjectMeathodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            byte[] newIL = new byte[IlBytesForReplaceMethod.Length];
            newIL[0] = 39;
            newIL[1] = 3;
            newIL[2] = 0;
            newIL[3] = 0;
            newIL[4] = 6;


            

            InjectionHelper.UpdateILCodes(ReplaceMethod,newIL);
            

        }
        /// <summary>
        /// DOES NOT WORK, DO NOT USE.
        /// </summary>
        /// <param name="typeToReplace"></param>
        /// <param name="ReplaceMeathodName"></param>
        /// <param name="typeToInjectWith"></param>
        /// <param name="InjectMeathodName"></param>
        public static void inject(Type typeToReplace, string ReplaceMeathodName, Type typeToInjectWith, string InjectMeathodName)
        {
            MethodInfo methodToReplace = typeToReplace.GetMethod(ReplaceMeathodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo methodToInject = typeToInjectWith.GetMethod(InjectMeathodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("\nVersion x86 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x86 Release\n");
                    *tar = *inj;
#endif
                }
                else
                {

                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;


                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
#endif
                }
            }
        }
    }*/
}
    
