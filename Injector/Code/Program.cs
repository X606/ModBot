using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.IO;
using Mono.Collections.Generic;
using ModLibrary;
using InternalModBot;

class Program
{
    public const string InjectionClassesNamespaceName = "InjectionClasses.";
    static void Main(string[] args)
    {
        string path = Environment.CurrentDirectory;
        string installPath = path + "/Assembly-CSharp.dll";
        string sourceToCopyClassesFrom = path + "/InjectionClasses.dll";
        string modlibrary = path + "/ModLibrary.dll";
        InstallModBot(installPath, sourceToCopyClassesFrom, modlibrary);
        
        Console.WriteLine("All injections completed!");
        
        System.Threading.Thread.Sleep(2000);
        

    }
    
    static void InstallModBot(string installPath, string sourceToCopyClassesFrom, string modLibraryPath)
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
        Injection refreshUpgradesInjection = Injector.AddCallToMethodInMethod(installPath, "FirstPersonMover", "RefreshUpgrades", modLibraryPath, "InternalModBot.CalledFromInjections", "FromRefreshUpgradesStart");
        refreshUpgradesInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        refreshUpgradesInjection.Write();

        Console.WriteLine("Injecting into Character.Start...");
        Injection onCharacterStartInjection = Injector.AddCallToMethodInMethod(installPath, "Character", "Start", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterStart");
        onCharacterStartInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        onCharacterStartInjection.Write();

        Console.WriteLine("Injecting into Character.Update...");
        Injection onCharacterUpdateInjection = Injector.AddCallToMethodInMethod(installPath, "Character", "Update", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterUpdate");
        onCharacterUpdateInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        onCharacterUpdateInjection.Write();

        Console.WriteLine("Injecting into Character.onDeath...");
        Injection onCharacterDeathInjection = Injector.AddCallToMethodInMethod(installPath, "Character", "onDeath", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnCharacterDeath");
        onCharacterDeathInjection.AddInstructionOverSafe(OpCodes.Ldarg_2);
        onCharacterDeathInjection.AddInstructionOverSafe(OpCodes.Ldarg_1);
        onCharacterDeathInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        onCharacterDeathInjection.Write();

        Console.WriteLine("Injecting into UpgradeDescription.GetAngleOffset...");
        Injection upgradeDescriptionInjection = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "GetAngleOffset", modLibraryPath, "InternalModBot.CalledFromInjections", "FromGetAngleOffset");
        upgradeDescriptionInjection.AddInstructionUnderSafe(OpCodes.Ret);
        upgradeDescriptionInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        upgradeDescriptionInjection.Write();

        Console.WriteLine("Injecting into UpgradeDescription.IsUpgradeCurrentlyVisible...");
        Injection upgradeDescriptionInjection2 = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "IsUpgradeCurrentlyVisible", modLibraryPath, "InternalModBot.CalledFromInjections", "FromIsUpgradeCurrentlyVisible");
        upgradeDescriptionInjection2.AddInstructionUnderSafe(OpCodes.Ret,0);
        upgradeDescriptionInjection2.AddInstructionOverSafe(OpCodes.Ldarg_0);
        upgradeDescriptionInjection2.Write();

        Console.WriteLine("Injecting into ErrorManager.HandleLog...");
        Injection errorManagerInjection = Injector.AddCallToMethodInMethod(installPath, "ErrorManager", "HandleLog", modLibraryPath, "InternalModBot.IgnoreCrashesManager", "GetIsIgnoringCrashes");
        errorManagerInjection.AddInstructionUnderSafe(OpCodes.Ret);
        errorManagerInjection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 3, 0, true);
        errorManagerInjection.Write();
        
        Console.WriteLine("Injecting into Projectile.FixedUpdate...");
        Injection ProjectileInjection1 = Injector.AddCallToMethodInMethod(installPath, "Projectile", "FixedUpdate", modLibraryPath, "InternalModBot.CalledFromInjections", "FromFixedUpdate");
        ProjectileInjection1.AddInstructionOverSafe(OpCodes.Ldarg_0);
        ProjectileInjection1.Write();

        Console.WriteLine("Injecting into Projectile.StartFlying...");
        Injection ProjectileInjection2 = Injector.AddCallToMethodInMethod(installPath, "Projectile", "StartFlying", modLibraryPath, "InternalModBot.CalledFromInjections", "FromStartFlying", 0, false, true, new string[] { "Vector3" ,"Vector3", "Boolean", "Character", "Int32", "Single"});
        ProjectileInjection2.AddInstructionOverSafe(OpCodes.Ldarg_0, ProjectileInjection2.GetLengthOfInstructions() - 2);
        ProjectileInjection2.Write();

        Console.WriteLine("Injecting into Projectile.DestroyProjectile...");
        Injection ProjectileInjection3 = Injector.AddCallToMethodInMethod(installPath, "Projectile", "DestroyProjectile", modLibraryPath, "InternalModBot.CalledFromInjections", "FromDestroyProjectile");
        ProjectileInjection3.AddInstructionOverSafe(OpCodes.Ldarg_0);
        ProjectileInjection3.Write();

        Console.WriteLine("Injecting into Projectile.OnEnvironmentCollided...");
        Injection ProjectileInjection4 = Injector.AddCallToMethodInMethod(installPath, "Projectile", "OnEnvironmentCollided", modLibraryPath, "InternalModBot.CalledFromInjections", "FromOnEnvironmentCollided");
        ProjectileInjection4.AddInstructionOverSafe(OpCodes.Ldarg_0);
        ProjectileInjection4.Write();

        Console.WriteLine("Injecting into GameUIRoot.RefreshCursorEnabled...");
        Injection GameUIRoot1 = Injector.AddCallToMethodInMethod(installPath, "GameUIRoot", "RefreshCursorEnabled", modLibraryPath, "InternalModBot.CalledFromInjections", "FromRefreshCursorEnabled");
        GameUIRoot1.AddInstructionUnderSafe(OpCodes.Ret);
        GameUIRoot1.AddInstructionUnderSafe(OpCodes.Brfalse_S, 3, 0, true);
        GameUIRoot1.Write();
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