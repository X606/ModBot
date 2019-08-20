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
        //peverify /IL "D:/games/new/steamapps/common/Clone Drone in the Danger Zone/Clone Drone in the Danger Zone_Data/Managed/Assembly-CSharp.dll"
        string path = "D:/games/new/steamapps/common/Clone Drone in the Danger Zone/Clone Drone in the Danger Zone_Data/Managed/Assembly-CSharp.dll";
        string path2 = "D:/games/new/steamapps/common/Clone Drone in the Danger Zone/Clone Drone in the Danger Zone_Data/Managed/InjectionClasses.dll";
        string path3 = "D:/games/new/steamapps/common/Clone Drone in the Danger Zone/Clone Drone in the Danger Zone_Data/Managed/ModLibrary.dll";
        InstallModBot(path, path2, path3);
        



        Console.WriteLine("All injections completed!");

        Console.ReadLine();
        //System.Threading.Thread.Sleep(2000);
        

    }
    
    static void InstallModBot(string installPath, string injectionSourceDllLocation, string modLibraryPath)
    {
        Console.WriteLine("Starting the installation of Mod-Bot...");
        if (!File.Exists(injectionSourceDllLocation))
        {
            ErrorHandler.Crash("Could not find dll at path \"" + injectionSourceDllLocation + "\"");
            return;
        }
        Console.WriteLine("Finding classes to inject...");
        ModuleDefinition module = ModuleDefinition.ReadModule(injectionSourceDllLocation, new ReaderParameters()
        {
            ReadWrite = true
        });

        List<TypeDefinition> types = module.GetTypes().ToList();
        List<string> typesFullNames = new List<string>();
        List<string> typesNames = new List<string>();
        for (int i = 0; i < types.Count; i++)
        {
            typesFullNames.Add(types[i].FullName);
            typesNames.Add(types[i].Name);
        }
        module.Dispose();
        Console.WriteLine("Found all classes to inject.");

        Console.WriteLine("Injecting classes...");
        
        for (int i = 0; i < typesFullNames.Count; i++)
        {
            Injector.AddClassToAssembly(installPath, typesNames[i], injectionSourceDllLocation, typesFullNames[i]);
        }
        Console.WriteLine("Finished injecting classes");

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

        Console.WriteLine("Injecting into UpgradeDescription.GetSkillPointCost...");
        Injection upgradeDescriptionInjection = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "GetSkillPointCost", modLibraryPath, "InternalModBot.CalledFromInjections", "FromGetSkillPointCost");
        upgradeDescriptionInjection.AddInstructionUnderSafe(OpCodes.Ret);
        upgradeDescriptionInjection.AddInstructionOverSafe(OpCodes.Ldarg_0);
        upgradeDescriptionInjection.Write();

        Console.WriteLine("Injecting into UpgradeDescription.IsUpgradeCurrentlyVisible...");
        Injection upgradeDescriptionInjection2 = Injector.AddCallToMethodInMethod(installPath, "UpgradeDescription", "IsUpgradeCurrentlyVisible", modLibraryPath, "InternalModBot.CalledFromInjections", "FromIsUpgradeCurrentlyVisible");
        upgradeDescriptionInjection2.AddInstructionUnderSafe(OpCodes.Ret,0);
        upgradeDescriptionInjection2.AddInstructionOverSafe(OpCodes.Ldarg_0);
        upgradeDescriptionInjection2.Write();

        Console.WriteLine("Injecting into RepairUpgrade.IsUpgradeCurrentlyVisible...");
        Injection upgradeDescriptionInjection3 = Injector.AddCallToMethodInMethod(installPath, "RepairUpgrade", "IsUpgradeCurrentlyVisible", modLibraryPath, "InternalModBot.CalledFromInjections", "FromIsRepairUpgradeCurrentlyVisible");
        upgradeDescriptionInjection3.AddInstructionUnderSafe(OpCodes.Ret);
        upgradeDescriptionInjection3.AddInstructionOverSafe(OpCodes.Ldarg_0);
        upgradeDescriptionInjection3.Write();

        Console.WriteLine("Injecting into ErrorManager.HandleLog...");
        Injection errorManagerInjection = Injector.AddCallToMethodInMethod(installPath, "ErrorManager", "HandleLog", modLibraryPath, "InternalModBot.IgnoreCrashesManager", "GetIsIgnoringCrashes");
        errorManagerInjection.AddInstructionUnderSafe(OpCodes.Ret);
        errorManagerInjection.AddInstructionUnderSafe(OpCodes.Brfalse_S, 3, 0, true);
        errorManagerInjection.Write();

        // TODO: reminder to add MultiplayerMatchManager.OnEvent(GenericStringForModdingEvent genericModdingEvent) here when the update comes out
        
        Console.WriteLine("Injecting into Projectile.SetInactive...");
        Injection ProjectileInjection1 = Injector.AddCallToMethodInMethod(installPath, "Projectile", "SetInactive", modLibraryPath, "InternalModBot.CalledFromInjections", "FromSetInactive");
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

    }



    static void debugInstall()
    {
        /*Injection injection = Injector.AddCallToMethodInMethod(path, "FirstPersonMover", "Awake", typeof(TestInjection).GetMethod("Test4"));
        if (injection != null)
        {
            Instruction instruction = injection.Processor.Create(OpCodes.Ldarg_0);
            injection.Processor.InsertBefore(injection.Processor.Body.Instructions[0], instruction);
            injection.Write();
        }


        Injection injection2 = Injector.AddCallToMethodInMethod(path, "FirstPersonMover", "Update", typeof(TestInjection).GetMethod("Test4"));
        if (injection2 != null)
        {
            Instruction instruction = injection.Processor.Create(OpCodes.Ldarg_0);
            injection2.Processor.InsertBefore(injection2.Processor.Body.Instructions[0], instruction);
            injection2.Write();
        }

        Injector.OverrideMethod(path, "Character", "IsAlive", path2, "TestInjection", "IsAlive");
        Injector.AddMethodToClass(path, "Character", "Test3", path2, "TestInjection", "Test3");
        Injector.AddFieldToClass(path, "Character", "Player", path2, "TestInjection", "Player");
        Injector.AddFieldToClass(path, "Character", "a", path2, "TestInjection", "a");
        Injector.AddFieldToClass(path, "Character", "testField2", path2, "TestInjection", "testField2");

        Injector.AddClassToAssembly(path, "test", path2, "testNamespace.testClass");*/
    }
}






public static class ErrorHandler
{
    public static void Crash(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("ERROR: " + errorMessage);
        Console.ForegroundColor = ConsoleColor.White;
        System.Threading.Thread.Sleep(10000);
        Environment.Exit(1);
    }
}