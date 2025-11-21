# Getting started

Before you can start modding you'll only need Visual Studio if you wont be including custom assets or viewing the game's 'decompiled' code.

<div class="programSeperator">

1\. [Visual Studio](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

Visual studio will be used to write and compile the code of your mod.  

NOTE: Make sure to include the '.NET Library' package during installation.

</div>

<div class="programSeperator">

2\. [Unity 2019.4.41](https://unity3d.com/get-unity/download/archive)

You will need to install the same version of Unity that Clone Drone has if you want to include custom assets in your mod (i.e. A custom model, animation, etc.).  

NOTE: It needs to be the exact version of Unity that Clone Drone uses (2019.4.41) or the game will crash when trying to import the custom assets.

</div>

<div class="programSeperator">

3\. [dnSpy](https://github.com/0xd4d/dnSpy/releases)

dnSpy is not necessary to make mods, the reason it's included in this list is because it 'decompiles' the game's code so looking up how something is done in the game or getting the name of a private member to use in the Accessor class is made easier.

</div>

## Setup

Once you are done installing Visual Studio and any other listed program above, you can now start to make a mod.

First, create a new '.NET Framework' project, name it, and click 'OK'

![Create new project preview image](https://cdn.discordapp.com/attachments/418510776215535640/524963150316044308/unknown.png)

When the project is created you will need to add references to some assemblies.

To do this, go to the Solution Explorer, located on the right of the window by default, and right-click on the item called 'References' and click on 'Add Reference...'

![Right click on references](https://cdn.discordapp.com/attachments/418510776215535640/524963864123539486/unknown.png)

When you do this, a window will open. Click on 'Browse...'

![Click on add](https://cdn.discordapp.com/attachments/418510776215535640/524964116369244170/unknown.png)

Navigate to your Clone Drone game files ([How do I find it?](https://steamcommunity.com/sharedfiles/filedetails/?id=760447682)), go into 'Clone Drone in the Danger Zone_Data' and then 'Managed', in that folder, find and select the following files:  
'Assembly-CSharp.dll', 'bolt.dll', 'bolt.user.dll', 'ModLibrary.dll', 'UnityEngine.CoreModule.dll', and 'UnityEngine.dll'.  
Then click 'Add', and then 'OK'

![Done button](https://cdn.discordapp.com/attachments/526159007442927648/547849278526062656/MBDAddAssemblies.PNG)  

Now add the following two lines of code at the top of your code:  
using ModLibrary;  
using UnityEngine;

Change 'public class Class1' to 'public class main : Mod' and give it the [MainModClass] attribute

![setup main class](https://media.discordapp.net/attachments/418510776215535640/1129841713842749470/image.png)  

You now have a blank canvas for a mod, to see a list of methods you can override, type in 'override' somewhere between the brackets under the main class.

![setup main class](https://media.discordapp.net/attachments/418510776215535640/1129841811985268766/image.png)

This lets you hook into different events in the game, some particularly useful ones being OnModRefreshed, GlobalUpdate, ImplementsSettingsWindow and CreateSettingsWindow.
A quick rundown on what these do:
* OnModRefreshed Will be called when your mod is loaded or refreshed, use to "reset" your mod
* GlobalUpdate is just called once every frame, and is the easiest way to eg take user input or similar
* ImplementsSettingsWindow just tells Mod-Bot if your mod has a "settings window", which is a little UI that lets you set up mod specific settings, if ImplementsSettingsWindow returns true CreateSettingsWindow will be called to actually specify what settings you want on your mod
* CreateSettingsWindow gets called when the user opens your mods settings window, use the passed builder to build up the settings you want.

Example of CreateSettingsWindow which will add a simple button that prints "I got clicked!" in the console and close the settings window when clicked:
```c#
protected override bool ImplementsSettingsWindow() => true;

protected override void CreateSettingsWindow(ModOptionsWindowBuilder builder)
{
    var page = builder.AddPage("Settings");
    page.AddButton("Click me!", delegate
    {
        debug.Log("I got clicked!");
        builder.ForceCloseWindow();
    });
}
```

Once you have something you're ready to test ingame, compile the mod by right clicking on your mods solution and clicking build
[!build mod](https://media.discordapp.net/attachments/418510776215535640/1129836093500575814/image.png)

Then create a new folder in your mods folder in your clone drone files (see above to find them). And copy the output dll from building your mod into it.

You can now also place in a image file that'll work as your mods thumbnail.

Finally, all mods have to come packaged with a ModInfo.json file, this file specifies metadata about your mod (eg the name of it)
For example:
```json
{
  "DisplayName": "My cool mod",
  "UniqueID": "e06c96b2-2c6d-4f78-a172-60666eb83050",
  "MainDLLFileName": "MyCoolMod.dll",
  "Author": "Mod Person",
  "Version": 0,
  "ImageFileName": "Image.png",
  "Description": "Adds something cool to the game!",
  "ModDependencies": [],
  "Tags": []
}
```
Make sure MainDLLFileName and ImageFileName exactly match the names of the other files.

Of course if you have any questions ask away in the discord!
