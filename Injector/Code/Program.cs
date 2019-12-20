using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.IO;
using Mono.Collections.Generic;

class Program
{
    public const string InjectionClassesNamespaceName = "InjectionClasses.";
    static void Main(string[] args)
    {
        try
        {
            string path = Environment.CurrentDirectory;
            string installPath = path + "/Assembly-CSharp.dll";
            string sourceToCopyClassesFrom = path + "/InjectionClasses.dll";
            string modlibrary = path + "/ModLibrary.dll";

            InstallModBot(installPath, sourceToCopyClassesFrom, modlibrary, path);

            Console.WriteLine("All injections completed!");

            System.Threading.Thread.Sleep(2000);
        }
        catch (Exception e)
        {
            ErrorHandler.Crash(e.ToString());
        }

    }

    static void InstallModBot(string installPath, string sourceToCopyClassesFrom, string modLibraryPath, string baseManagedPath)
    {
        Console.WriteLine("Starting the installation of Mod-Bot...");
        if (!File.Exists(sourceToCopyClassesFrom))
        {
            ErrorHandler.Crash("Could not find dll at path \"" + sourceToCopyClassesFrom + "\"");
            return;
        }
        Console.WriteLine("Finding classes to inject...");
        ModuleDefinition module = ModuleDefinition.ReadModule(sourceToCopyClassesFrom, new ReaderParameters()
        {
            ReadWrite = true
        });

        List<TypeDefinition> types = module.GetTypes().ToList();
        List<string> typesFullNames = new List<string>();
        List<string> typesNames = new List<string>();
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i].Namespace != "InjectionClasses")
                continue;

            typesFullNames.Add(types[i].FullName);
            typesNames.Add(types[i].Name);
        }
        module.Dispose();
        Console.WriteLine("Found all classes to inject.");

        Console.WriteLine("Injecting classes...");
        
        for (int i = 0; i < typesFullNames.Count; i++)
        {
            Injector.AddClassToAssembly(installPath, typesNames[i], sourceToCopyClassesFrom, typesFullNames[i]);
        }
        Console.WriteLine("Finished injecting classes");

        Console.WriteLine("Injection new methods...");

        Console.WriteLine("Injecting GetPositionForAIToAimAt into MortarWalker");
        Injector.AddMethodToClass(installPath, "MortarWalker", "GetPositionForAIToAimAt", sourceToCopyClassesFrom, "FixSpidertrons", "GetPositionForAIToAimAt");
        Console.WriteLine("Injected!");

        Console.WriteLine("Done injecting new methods!");

        Console.WriteLine("Injecting method calls...");

        Console.WriteLine("Injecting into GameFlowManager.Start...");
        Injector.AddCallToMethodInMethod(installPath, "GameFlowManager", "Start", modLibraryPath, "InternalModBot.StartupManager", "OnStartUp").Write();

        Console.WriteLine("Injecting into FirstPersonMover.RefreshUpgrades...");
        Injection FirstPersonMover_RefreshUpgrades_Injection = Injector.AddCallToMethodInMethod(installPath, "FirstPersonMover", "RefreshUpgrades", modLibraryPath, "InternalModBot.CalledFromInjections", "FromRefreshUpgradesStart");
        FirstPersonMover_RefreshUpgrades_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        FirstPersonMover_RefreshUpgrades_Injection.Write();

        Console.WriteLine("Injecting into FirstPersonMover.ExecuteCommand...");
        Injection FirstPersonMover_ExecuteCommand_Injection = Injector.AddCallToMethodInMethod(installPath, "FirstPersonMover", "ExecuteCommand", modLibraryPath, "InternalModBot.CalledFromInjections", "FromExecuteCommand", 2, true);
        FirstPersonMover_ExecuteCommand_Injection.AddInstructionUnderSafe(OpCodes.Stloc_0);
        FirstPersonMover_ExecuteCommand_Injection.AddInstructionOverSafe(OpCodes.Ldarg_1);
        FirstPersonMover_ExecuteCommand_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        FirstPersonMover_ExecuteCommand_Injection.Write();

        Console.WriteLine("Injecting into Character.Start...");
        Injection Character_Start_Injection = Injector.AddCallToMethodInMethod(installPath, "Character", "Start", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterStart");
        Character_Start_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Character_Start_Injection.Write();

        Console.WriteLine("Injecting into Character.Update...");
        Injection Character_Update_Injection = Injector.AddCallToMethodInMethod(installPath, "Character", "Update", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterUpdate");
        Character_Update_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Character_Update_Injection.Write();

        Console.WriteLine("Injecting into Character.onDeath...");
        Injection Character_onDeath_Injection = Injector.AddCallToMethodInMethod(installPath, "Character", "onDeath", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterDeath");
        Character_onDeath_Injection.AddInstructionOverSafe(OpCodes.Ldarg_2);
        Character_onDeath_Injection.AddInstructionOverSafe(OpCodes.Ldarg_1);
        Character_onDeath_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Character_onDeath_Injection.Write();

        Console.WriteLine("Injecting into UpgradeDescription.GetAngleOffset...");
        Injection UpgradeDescription_GetAngleOffset_Injection = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "GetAngleOffset", modLibraryPath, "InternalModBot.CalledFromInjections", "FromGetAngleOffset");
        UpgradeDescription_GetAngleOffset_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        UpgradeDescription_GetAngleOffset_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        UpgradeDescription_GetAngleOffset_Injection.Write();

        Console.WriteLine("Injecting into UpgradeDescription.IsUpgradeCurrentlyVisible...");
        Injection UpgradeDescription_IsUpgradeCurrentlyVisible_Injection = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "IsUpgradeCurrentlyVisible", modLibraryPath, "InternalModBot.CalledFromInjections", "FromIsUpgradeCurrentlyVisible");
        UpgradeDescription_IsUpgradeCurrentlyVisible_Injection.AddInstructionUnderSafe(OpCodes.Ret,0);
        UpgradeDescription_IsUpgradeCurrentlyVisible_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        UpgradeDescription_IsUpgradeCurrentlyVisible_Injection.Write();

        Console.WriteLine("Injecting into ErrorManager.HandleLog...");
        Injection ErrorManager_HandleLog_Injection = Injector.AddCallToMethodInMethod(installPath, "ErrorManager", "HandleLog", modLibraryPath, "InternalModBot.IgnoreCrashesManager", "GetIsIgnoringCrashes");
        ErrorManager_HandleLog_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        ErrorManager_HandleLog_Injection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 3, 0, true);
        ErrorManager_HandleLog_Injection.Write();
        
        Console.WriteLine("Injecting into Projectile.FixedUpdate...");
        Injection Projectile_FixedUpdate_Injection = Injector.AddCallToMethodInMethod(installPath, "Projectile", "FixedUpdate", modLibraryPath, "InternalModBot.CalledFromInjections", "FromFixedUpdate");
        Projectile_FixedUpdate_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Projectile_FixedUpdate_Injection.Write();

        Console.WriteLine("Injecting into Projectile.StartFlying...");
        Injection Projectile_StartFlying_Injection = Injector.AddCallToMethodInMethod(installPath, "Projectile", "StartFlying", modLibraryPath, "InternalModBot.CalledFromInjections", "FromStartFlying", 0, false, true, new string[] { "Vector3" ,"Vector3", "Boolean", "Character", "Int32", "Single"});
        Projectile_StartFlying_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0, Projectile_StartFlying_Injection.GetLengthOfInstructions() - 2);
        Projectile_StartFlying_Injection.Write();

        Console.WriteLine("Injecting into Projectile.DestroyProjectile...");
        Injection Projectile_DestroyProjectile_Injection = Injector.AddCallToMethodInMethod(installPath, "Projectile", "DestroyProjectile", modLibraryPath, "InternalModBot.CalledFromInjections", "FromDestroyProjectile");
        Projectile_DestroyProjectile_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Projectile_DestroyProjectile_Injection.Write();

        Console.WriteLine("Injecting into Projectile.OnEnvironmentCollided...");
        Injection Projectile_OnEnvironmentCollided_Injection = Injector.AddCallToMethodInMethod(installPath, "Projectile", "OnEnvironmentCollided", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnEnvironmentCollided");
        Projectile_OnEnvironmentCollided_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Projectile_OnEnvironmentCollided_Injection.Write();

        Console.WriteLine("Injecting into GameUIRoot.RefreshCursorEnabled...");
        Injection GameUIRoot_RefreshCursorEnabled_Injection = Injector.AddCallToMethodInMethod(installPath, "GameUIRoot", "RefreshCursorEnabled", modLibraryPath, "InternalModBot.CalledFromInjections", "FromRefreshCursorEnabled");
        GameUIRoot_RefreshCursorEnabled_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        GameUIRoot_RefreshCursorEnabled_Injection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 3, 0, true);
        GameUIRoot_RefreshCursorEnabled_Injection.Write();

        Console.WriteLine("Injecting into Resources.Load...");
        Injection Resources_Load_Injection = Injector.AddCallToMethodInMethod(baseManagedPath + "/UnityEngine.CoreModule.dll", "UnityEngine.Resources", "Load", modLibraryPath, "InternalModBot.CalledFromInjections", "FromResourcesLoad");
        Resources_Load_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        Resources_Load_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        Resources_Load_Injection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 4, 0, true);
        Resources_Load_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        Resources_Load_Injection.AddInstructionUnderSafe(OpCodes.Stloc_0);
        Resources_Load_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Resources_Load_Injection.Write();

        Console.WriteLine("Injecting into Resources.Load<T>...");
        Injection Resources_LoadT_Injection = Injector.AddCallToMethodInMethod(baseManagedPath + "/UnityEngine.CoreModule.dll", "UnityEngine.Resources", "Load", modLibraryPath, "InternalModBot.CalledFromInjections", "FromResourcesLoad", 0, false, false, null, true);
        Resources_LoadT_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        Resources_LoadT_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        Resources_LoadT_Injection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 4, 0, true);
        Resources_LoadT_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        Resources_LoadT_Injection.AddInstructionUnderSafe(OpCodes.Stloc_0);
        Resources_LoadT_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        Resources_LoadT_Injection.Write();

        Console.WriteLine("Injecting into ResourceRequest.get_asset...");
        Injection ResourceRequest_get_asset_Injection = Injector.AddCallToMethodInMethod(baseManagedPath + "/UnityEngine.CoreModule.dll", "UnityEngine.ResourceRequest", "get_asset", modLibraryPath, "InternalModBot.CalledFromInjections", "FromResourcesLoad");
        ResourceRequest_get_asset_Injection.AddInstructionUnderSafe(OpCodes.Ret);
        ResourceRequest_get_asset_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        ResourceRequest_get_asset_Injection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 4, 0, true);
        ResourceRequest_get_asset_Injection.AddInstructionUnderSafe(OpCodes.Ldloc_0);
        ResourceRequest_get_asset_Injection.AddInstructionUnderSafe(OpCodes.Stloc_0);
        ResourceRequest_get_asset_Injection.AddInstructionOverSafe(OpCodes.Ldfld, ResourceRequest_get_asset_Injection.GetFieldReferenceOnSameType("m_Path"));
        ResourceRequest_get_asset_Injection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        ResourceRequest_get_asset_Injection.Write();

        Console.WriteLine("Injecting into LocalizationManager.populateDictionaryForCurrentLanguage...");
        Injection LocalizationManager_populateDictionaryForCurrentLanguage_Injection = Injector.AddCallToMethodInMethod(installPath, "LocalizationManager", "populateDictionaryForCurrentLanguage", modLibraryPath, "InternalModBot.CalledFromInjections", "FromPopulateLanguageDictionary", 0, false, true);
        LocalizationManager_populateDictionaryForCurrentLanguage_Injection.Write();

        Console.WriteLine("Appying the melon patch (unbans melon)");
        Injector.MelonPatch(installPath); // unbans melon
    }
}

public static class ErrorHandler
{
    public static void Crash(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("ERROR: " + errorMessage);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("WARNING: Your game files may have been corrupted, please verify your game files before starting the game!!!");
        Console.ForegroundColor = ConsoleColor.White;
        System.Threading.Thread.Sleep(30 * 60 * 1000);
        Environment.Exit(1);
    }
}