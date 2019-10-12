# Getting started

Before you can start modding you'll only need Visual Studio if you wont be including custom assets or viewing the game's 'decompiled' code.

<div class="programSeperator">

1\. [Visual Studio](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

Visual studio will be used to write and compile the code of your mod.  

NOTE: Make sure to include the '.NET Library' package during installation.

</div>

<div class="programSeperator">

2\. [Unity 2018.3.11](https://unity3d.com/get-unity/download/archive)

You will need to install the same version of Unity that Clone Drone has if you want to include custom assets in your mod (i.e. A custom model, animation, etc.).  

NOTE: It needs to be the exact version of Unity that Clone Drone uses (2018.3.11) or the game will crash when trying to import the custom assets.

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

Change 'public class Class1' to 'public class main : Mod'

![setup main class](https://cdn.discordapp.com/attachments/418510776215535640/524964594662244362/unknown.png)  

You now have a blank canvas for a mod, to see a list of methods you can override, type in 'public override' somewhere between the brackets under the main class.

![setup main class](https://cdn.discordapp.com/attachments/418510776215535640/524965033269133342/unknown.png)

Finaly, all mods should override GetModName and GetUniqueID, this is to make sure that the user can see what mod is what in game.

And of course if you have any questions ask away in the discord!
